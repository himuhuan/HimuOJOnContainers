using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

public class Submission : Entity, IAggregateRoot
{
    public int? ProblemId { get; private set; }

    public string SubmitterId { get; private set; }

    [Required]
    public string SourceCode { get; private set; }

    [Required]
    public DateTime SubmitTime { get; private set; }

    /// <summary>
    /// <see cref="CompilerName"/> is the name of the compiler used to compile the source code.
    /// <seealso cref="SupportedCompiler"/>
    /// </summary>
    [Required]
    public string CompilerName { get; private set; }

    public ResourceUsage Usage { get; private set; }

    /// <summary>
    /// Status depends on the results of each test points
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JudgeStatus Status { get; private set; }

    /// <summary>
    /// Message from the compiler or the background service.
    /// </summary>
    /// <remarks>
    /// If the submission is accepted, this message should be null.
    /// </remarks>
    public string StatusMessage { get; private set; }

    [JsonIgnore]
    private readonly List<TestPointResult> _testPointResults;

    public IReadOnlyCollection<TestPointResult> TestPointResults => _testPointResults.AsReadOnly();

    /// <remarks>
    /// Generally speaking, DDD requires us not to expose this constructor to public,
    /// But this type requires the serialization and deserialization of integrated events.
    /// </remarks>
    public Submission()
    {
        _testPointResults = [];
    }

    public Submission(int problemId, string submitterId, string sourceCode, DateTime submitTime, string compilerName)
        : this()
    {
        Status       = JudgeStatus.Pending;
        ProblemId    = problemId;
        SubmitterId  = submitterId;
        SourceCode   = sourceCode;
        SubmitTime   = submitTime;
        CompilerName = compilerName;
    }

    // TODO: Perhaps it would be better to set the Status Message as a field. 
    private void SetMessage(string message)
    {
        const int maxLength = 10000;
        if (message != null)
        {
            StatusMessage = message.Length > maxLength
                ? string.Concat(message.AsSpan(0, maxLength - 3), "...")
                : message;
        }
    }

    public void UpdateStatus(JudgeStatus status, string message = null)
    {
        Status = status;
        SetMessage(message);
    }

    public void UpdateStatus(int testPointId, JudgeStatus status, string message)
    {
        int index = _testPointResults.FindIndex(r => r.TestPointId == testPointId);
        if (index == -1)
        {
            throw new InvalidOperationException("Test point result not found.");
        }

        _testPointResults[index].UpdateStatus(status);
        if (status != JudgeStatus.Accepted)
        {
            Status = status;
            SetMessage(message);
        }
    }

    public void UpdateStatus(int testResultId, OutputDifference difference)
    {
        int index = FindIndexOfTestPoint(testResultId);
        if (index == -1)
        {
            throw new InvalidOperationException("Test point result not found.");
        }

        _testPointResults[index]
            .UpdateDifference(difference.ExpectedOutput, difference.ActualOutput, difference.Position);
        Status        = JudgeStatus.WrongAnswer;
        StatusMessage = "Output difference detected";
    }

    public void UpdateResultResourceUsage(int testPointId, ResourceUsage usage)
    {
        int index = FindIndexOfTestPoint(testPointId);
        if (index == -1)
        {
            throw new InvalidOperationException("Test point result not found.");
        }

        _testPointResults[index].UpdateUsage(usage);
        Usage ??= new ResourceUsage();
        Usage.Update(usage.UsedMemoryByte, usage.UsedTimeMs);
    }

    public void AddTestPointResult(TestPointResult result)
    {
        _testPointResults.Add(result);
    }

    private int FindIndexOfTestPoint(int testPointId)
    {
        int low = 0, high = _testPointResults.Count;
        while (low < high)
        {
            int mid = low + (high - low) / 2;
            if (_testPointResults[mid].TestPointId == testPointId)
                return mid;
            if (_testPointResults[mid].TestPointId < testPointId)
                low = mid + 1;
            else
                high = mid;
        }

        return -1;
    }
}