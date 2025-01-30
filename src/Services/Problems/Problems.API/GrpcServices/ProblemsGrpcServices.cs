#region

using Grpc.Core;
using GrpcProblems;
using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Problems.API.Application.Services;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;

#endregion

namespace HimuOJ.Services.Problems.API.GrpcServices;

public class ProblemsGrpcServices : ProblemsService.ProblemsServiceBase
{
    private readonly ILogger<ProblemsGrpcServices> _logger;
    private readonly IProblemsRepository _repository;
    private readonly IResourceStorage _resource;

    public ProblemsGrpcServices(
        IProblemsRepository repository,
        ILogger<ProblemsGrpcServices> logger,
        IResourceStorage resource)
    {
        _repository = repository;
        _logger = logger;
        _resource = resource;
    }

    [AllowAnonymous]
    public override async Task<GetProblemEssentialPartForJudgeResponse>
        GetProblemEssentialPartForJudge(
            GetProblemEssentialPartForJudgeRequest request,
            ServerCallContext context)
    {
        var problem = await _repository.GetAsync(request.ProblemId);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Begin GRPC call: {method} {@request}", context.Method, request);
        }

        if (problem == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Problem not found"));
        }

        var response = new GetProblemEssentialPartForJudgeResponse
        {
            MaxMemoryLimitByte = problem.DefaultResourceLimit.MaxMemoryLimitByte,
            MaxTimeLimitMs = problem.DefaultResourceLimit.MaxRealTimeLimitMilliseconds
        };

        foreach (TestPoint testPoint in problem.TestPoints)
        {
            response.TestPoints.Add(new TestPointEssentialPart
            {
                TestPointId = testPoint.Id,
                Input = (testPoint.ResourceType == TestPointResourceType.Text)
                    ? testPoint.Input : _resource.GetResourceUrl(problem.Id, testPoint.Input),
                ExpectedOutput = (testPoint.ResourceType == TestPointResourceType.Text) 
                    ? testPoint.ExpectedOutput : _resource.GetResourceUrl(problem.Id, testPoint.ExpectedOutput),
                OutdatedTimestamp = new DateTimeOffset(testPoint.LastModifyTime).ToUnixTimeSeconds(),
                ResourceType = testPoint.ResourceType switch
                {
                    TestPointResourceType.File => ResoureType.File,
                    _ => ResoureType.Text,
                }
            });
        }

        return response;
    }

    [AllowAnonymous]
    public override Task<CheckHealthResponse> CheckHealth(
        CheckHealthRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(new CheckHealthResponse
        {
            Status = 0
        });
    }
}