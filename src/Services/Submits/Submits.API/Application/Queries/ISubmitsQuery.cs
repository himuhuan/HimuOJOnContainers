using HimuOJ.Common.WebHostDefaults.Infrastructure;

namespace HimuOJ.Services.Submits.API.Application.Queries;

public interface ISubmitsQuery
{
    Task<ApiResult<ProblemSubmitStatistics>> GetProblemSubmitStatisticsAsync(int problemId);
    Task<GetSubmissionResult?> GetSubmissionAsync(int submissionId);
    Task<ApiResult<SubmissionList>> GetSubmissionListAsync(GetSubmissionsListRequest request);
    Task<ApiResult<UserProfileStatistics>> GetUserProfileStatisticsAsync(string userId);
}