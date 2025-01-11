#region

using GrpcProblems;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using HimuOJ.Services.Submits.Infrastructure.Repositories;
using MediatR;
using Submits.BackgroundTasks.Events;
using Submits.BackgroundTasks.Library;
using Submits.BackgroundTasks.Services.Remote;
using Submits.BackgroundTasks.Services.Sandbox;

#endregion

namespace Submits.BackgroundTasks.Services.Judge;

#region

using ProblemInfo = GetProblemEssentialPartForJudgeResponse;

#endregion

public class JudgeService : IJudgeService
{
    private readonly ILocalCacheFileService _cacheFiles;
    private readonly ICompileService _compileService;
    private readonly ILogger<JudgeService> _logger;
    private readonly IMediator _mediator;
    private readonly ProblemsServices _problemsServices;
    private readonly ISandboxService _sandbox;
    private readonly ISubmitsRepository _submitsRepository;

    public JudgeService(
        ILogger<JudgeService> logger,
        ProblemsServices problemsServices,
        ISubmitsRepository submitsRepository,
        ICompileService compileService,
        ISandboxService sandbox,
        ILocalCacheFileService cacheFiles,
        IMediator mediator)
    {
        _logger            = logger;
        _problemsServices  = problemsServices;
        _submitsRepository = submitsRepository;
        _compileService    = compileService;
        _sandbox           = sandbox;
        _cacheFiles        = cacheFiles;
        _mediator          = mediator;
    }

    public void AddJudgeTask(int submissionId)
    {
        _logger.LogInformation("Add judge task for submission {SubmissionId}", submissionId);

        // TODO: we start judge task directly here, and then consider judge task queue later
        JudgeTask task = new(submissionId);

        RunJudgeTask(task).Wait();
    }

    private async Task RunJudgeTask(JudgeTask task)
    {
        _logger.LogInformation("Starting judge task for submission {SubmissionId}",
            task.SubmissionId);

        int submissionId = task.SubmissionId;
        var submission   = await _submitsRepository.GetAsync(submissionId);
        if (submission == null)
        {
            _logger.LogCritical("Submission {SubmissionId} not found", submissionId);
            return;
        }

        if (!submission.ProblemId.HasValue)
        {
            await _mediator.Publish(new JudgeTaskExitedEvent(
                submission, JudgeStatus.SystemError,
                "The corresponding problem submitted has been removed"));
            return;
        }

        var problemPart =
            await _problemsServices.GetProblemEssentialPartForJudgeAsync(submission.ProblemId
                .Value);
        if (problemPart == null)
        {
            await _mediator.Publish(new JudgeTaskExitedEvent(submission, JudgeStatus.SystemError,
                "The corresponding problem submitted does not exist."));
            return;
        }

        await PrepareSubmission(submission, problemPart);

        CompilerResult? compilerResult = await CompileSourceCode(submission);
        if (compilerResult is not { ExitCode: 0 })
        {
            return;
        }

        string executable = compilerResult.Executable;
        bool   hasError   = false;
        foreach (var testPoint in problemPart.TestPoints)
        {
            _logger.LogDebug("Judging test point {TestPointId} for submission {SubmissionId}",
                testPoint.TestPointId,
                submissionId);
            if (!await RunTestPoint(testPoint, submission, executable, problemPart))
            {
                hasError = true;
                break;
            }

            _logger.LogDebug("Test point {TestPointId} for submission {SubmissionId} passed",
                testPoint.TestPointId,
                submissionId);
            submission.UpdateStatus(testPoint.TestPointId, JudgeStatus.Accepted, null);
        }

        if (hasError)
        {
            _logger.LogInformation(
                "There are some test points in submission {SubmissionId} has skipped",
                submissionId);
        }
        else
        {
            // All test points passed
            submission.UpdateStatus(JudgeStatus.Accepted);
            await _mediator.Publish(
                new JudgeTaskExitedEvent(submission, JudgeStatus.Accepted, null));
        }

        _logger.LogInformation("Judge task for submission {SubmissionId} finished", submissionId);
    }

    private async Task<bool> RunTestPoint(
        TestPointEssentialPart testPoint,
        Submission submission,
        string executable,
        ProblemInfo problemPart)
    {
        var inputPath = await _cacheFiles.CreateOrGetTextFileAsync(
            "input", testPoint.TestPointId + ".in", testPoint.Input);
        var outputPath = await _cacheFiles.CreateOrGetTextFileAsync(
            "output", testPoint.TestPointId + ".out", string.Empty);

        var sandboxResult =
            RunSandbox(submission.Id, executable, inputPath, outputPath, submission.CompilerName,
                problemPart);
        SandboxStatus status = (SandboxStatus) sandboxResult.Status;
        if (status != SandboxStatus.Success || sandboxResult.ExitCode != 0)
        {
            await _mediator.Publish(new JudgeTaskExitedEvent(submission,
                testPoint.TestPointId,
                MapStatusToJudgeStatus(status),
                MapStatusToMessage(status)));
            return false;
        }

        submission.UpdateResultResourceUsage(testPoint.TestPointId,
            new ResourceUsage((long) sandboxResult.MemoryUsage,
                (long) sandboxResult.RealTimeUsage));

        string expectedOutputPath = await _cacheFiles.CreateOrGetTextFileAsync(
            "answer", testPoint.TestPointId + ".ans", testPoint.ExpectedOutput);
        var outputDifference = await CompareOutput(expectedOutputPath, outputPath);
        if (outputDifference != null)
        {
            await _mediator.Publish(
                new JudgeTaskExitedEvent(submission, testPoint.TestPointId,
                    Status: JudgeStatus.WrongAnswer,
                    OutputDifference: outputDifference));
            return false;
        }

        return true;
    }

    private async Task PrepareSubmission(Submission submission, ProblemInfo part)
    {
        int submissionId = submission.Id;

        _logger.LogDebug("Preparing to judge submission {SubmissionId} for problem {ProblemId}",
            submissionId,
            submission.ProblemId);

        if (submission.Status != JudgeStatus.Pending)
        {
            _logger.LogWarning("Submission {SubmissionId} is not in pending status", submissionId);
        }
        else
        {
            foreach (TestPointEssentialPart testPoint in part.TestPoints)
            {
                submission.AddTestPointResult(new TestPointResult(testPoint.TestPointId));
            }
        }

        submission.UpdateStatus(JudgeStatus.Running);
        _submitsRepository.Update(submission);
        await _submitsRepository.UnitOfWork.SaveEntitiesAsync();
    }

    private async Task<CompilerResult?> CompileSourceCode(Submission submission)
    {
        int submissionId = submission.Id;
        _logger.LogDebug("Compiling submission {SubmissionId}", submissionId);
        var compilerResult =
            _compileService.Compile(submission.CompilerName, submission.SourceCode, submissionId);
        if (compilerResult == null)
        {
            await _mediator.Publish(new JudgeTaskExitedEvent(submission, JudgeStatus.SystemError,
                "Compiler not found"));
            return compilerResult;
        }

        if (compilerResult.ExitCode != 0)
        {
            await _mediator.Publish(new JudgeTaskExitedEvent(submission,
                JudgeStatus.CompilationError,
                compilerResult.Message));
        }

        return compilerResult;
    }

    private SandboxResult RunSandbox(
        long submissionId,
        string executable,
        string input,
        string output,
        string compilerName,
        ProblemInfo part)
    {
        SandboxConfiguration configuration = new(submissionId.ToString(), executable)
        {
            MaxMemory   = (ulong) part.MaxMemoryLimitByte,
            MaxRealTime = (ulong) part.MaxTimeLimitMs,
            InputFile   = input,
            OutputFile  = output,
            Policy      = MapCompilerNameToPolicy(compilerName)
        };

        return _sandbox.Run(configuration);
    }

    private async Task<OutputDifference?> CompareOutput(
        string expectedOutputPath,
        string actualOutputPath)
    {
        using StreamReader expectedReader = new(expectedOutputPath);
        using StreamReader actualReader   = new(actualOutputPath);

        int lineCount = 0;
        for (string? expectedLine = await expectedReader.ReadLineAsync();
             expectedLine != null;
             expectedLine = await expectedReader.ReadLineAsync())
        {
            string actualLine = (await actualReader.ReadLineAsync() ?? "").Trim();
            ++lineCount;
            if (expectedLine != actualLine)
            {
                return new OutputDifference(expectedLine, actualLine, lineCount);
            }
        }

        return null;
    }

    private static JudgeStatus MapStatusToJudgeStatus(SandboxStatus s)
    {
        return s switch
        {
            SandboxStatus.MemoryLimitExceeded   => JudgeStatus.MemoryLimitExceeded,
            SandboxStatus.CpuTimeLimitExceeded  => JudgeStatus.TimeLimitExceeded,
            SandboxStatus.RealTimeLimitExceeded => JudgeStatus.TimeLimitExceeded,
            SandboxStatus.InternalError         => JudgeStatus.SystemError,
            _                                   => JudgeStatus.RuntimeError
        };
    }

    private static string MapStatusToMessage(SandboxStatus sc)
    {
        return sc switch
        {
            SandboxStatus.MemoryLimitExceeded   => "Memory limit exceeded",
            SandboxStatus.CpuTimeLimitExceeded  => "CPU Time limit exceeded",
            SandboxStatus.RealTimeLimitExceeded => "REAL Time limit exceeded",
            SandboxStatus.InternalError         => "Internal error",
            SandboxStatus.IllegalOperation      => "Illegal operation",
            SandboxStatus.Success               => "Success",
            SandboxStatus.RuntimeError          => "Runtime error",
            SandboxStatus.ProcessLimitExceeded  => "Process limit exceeded",
            SandboxStatus.OutputLimitExceeded   => "Output limit exceeded",
            _                                   => throw new ArgumentOutOfRangeException(nameof(sc))
        };
    }

    private static int MapCompilerNameToPolicy(string compilerName)
    {
        // TODO: add more compiler policies
        return compilerName switch
        {
            // CXX_PROGRAM
            "g++"     => 1,
            "gcc"     => 1,
            "clang++" => 1,
            "clang"   => 1,

            // NO LIMITS
            _ => 0
        };
    }
}