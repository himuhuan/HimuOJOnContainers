using HimuOJ.Services.Submits.API.Application.Queries;
using HimuOJ.Web.WebSPA.Models;
using Refit;

namespace HimuOJ.Web.WebSPA.Services;

public interface ISubmitsApi
{
    [Get("/api/submissions/statistics/problems-list/{problemId}")]
    Task<ProblemSubmitStatistics> GetSubmitsStatisticsAsync(int problemId);
    
    [Get("/api/submissions/list")]
    Task<BffSubmissionList> GetSubmissionsListAsync([Query] GetSubmissionsListRequest request);

    [Get("/api/submissions/{id}")]
    Task<BffSubmission> GetSubmission(int id);
    
    [Get("/api/submissions/statistics/user-profile/{userId}")]
    Task<UserProfileStatistics> GetUserProfileStatisticsAsync(string userId);
}