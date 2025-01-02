using Grpc.Core;
using GrpcProblems;

namespace Submits.BackgroundTasks.Services.Remote;

using GrpcProblemsClient = ProblemsService.ProblemsServiceClient;

public class ProblemsServices
{
    private readonly GrpcProblemsClient _client;

    public ProblemsServices(GrpcProblemsClient client)
    {
        _client = client;
    }

    public async Task<GetProblemEssentialPartForJudgeResponse?> 
        GetProblemEssentialPartForJudgeAsync(int problemId)
    {
        var request = new GetProblemEssentialPartForJudgeRequest
        {
            ProblemId = problemId
        };

        try
        {
            return await _client.GetProblemEssentialPartForJudgeAsync(request);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }
}