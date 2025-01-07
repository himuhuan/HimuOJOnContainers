using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Services.Submits.API.Application.Queries;
using HimuOJ.Web.WebSPA.Services;
using Identity.Server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HimuOJ.Web.WebSPA.Controller
{
    [Route("api/bff")]
    [ApiController]
    public class BffController : ControllerBase
    {
        private readonly IProblemsApi _problemsApi;
        private readonly ISubmitsApi _submitsApi;
        private readonly IUsersApi _usersApi;

        public BffController(IProblemsApi problemsApi, ISubmitsApi submitsApi, IUsersApi usersApi)
        {
            _problemsApi = problemsApi;
            _submitsApi  = submitsApi;
            _usersApi    = usersApi;
        }

        [HttpGet("problems-list")]
        public async Task<IActionResult> GetProblemsListAsync([FromQuery] GetProblemsListRequest request)
        {
            var result = await _problemsApi.GetProblemsListAsync(request);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            await Parallel.ForEachAsync(result.Items, parallelOptions, async (item, _) =>
            {
                var statistics = await _submitsApi.GetSubmitsStatisticsAsync(item.Id);
                item.AcceptedSubmissionCount = statistics.AcceptedSubmits;
                item.TotalSubmissionCount    = statistics.TotalSubmits;
            });

            return Ok(result);
        }

        [HttpGet("submissions-list")]
        public async Task<IActionResult> GetSubmissionsListAsync([FromQuery] GetSubmissionsListRequest request)
        {
            var result = await _submitsApi.GetSubmissionsListAsync(request);

            var problemIds = result.Items
                                   .Select(x => x.ProblemId ?? -1)
                                   .Distinct()
                                   .ToList();
            var problemTitlesTask = _problemsApi.GetProblemTitlesAsync(
                new GetProblemTitleListRequest
                {
                    Ids = problemIds
                });

            var userIds = result.Items.Select(x => x.SubmitterId ?? string.Empty).Distinct().ToList();
            var userBriefsTask = _usersApi.GetUserBriefsAsync(
                new GetUserBriefsRequest
                {
                    Ids = userIds
                });

            await Task.WhenAll(userBriefsTask, problemTitlesTask);
            IDictionary<string, UserBrief> userBriefs    = userBriefsTask.Result;
            IDictionary<int, string>       problemTitles = problemTitlesTask.Result;

            // aggregate problem titles and user briefs
            foreach (var item in result.Items)
            {
                if (item.SubmitterId != null && userBriefs.TryGetValue(item.SubmitterId, out var userBrief))
                {
                    item.SubmitterName   = userBrief.UserName;
                    item.SubmitterAvatar = userBrief.Avatar;
                }

                if (item.ProblemId.HasValue && problemTitles.TryGetValue(item.ProblemId.Value, out var title))
                {
                    item.ProblemTitle = title;
                }
            }

            return Ok(result);
        }

        [HttpGet("submissions-detail/{id}")]
        public async Task<IActionResult> GetSubmissionDetail(int id)
        {
            var submission = await _submitsApi.GetSubmission(id);

            Task<UserBrief>? userBriefTask    = null;
            Task<string>?    problemTitleTask = null;

            if (submission.SubmitterId != null)
                userBriefTask = _usersApi.GetUserBriefAsync(submission.SubmitterId);
            if (submission.ProblemId.HasValue)
                problemTitleTask = _problemsApi.GetProblemTitleAsync(submission.ProblemId.Value);

            try
            {
                await Task.WhenAll(
                    userBriefTask ?? Task.CompletedTask,
                    problemTitleTask ?? Task.CompletedTask);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            submission.SubmitterName   = userBriefTask?.Result.UserName;
            submission.SubmitterAvatar = userBriefTask?.Result.Avatar;
            submission.ProblemTitle    = problemTitleTask?.Result;

            return Ok(submission);
        }

        [HttpGet("users-detail/{id}")]
        public async Task<IActionResult> GetUserDetail(string id)
        {
            var userDetailTask = _usersApi.GetUserDetailAsync(id);
            var profileTask    = _submitsApi.GetUserProfileStatisticsAsync(id);

            await Task.WhenAll(userDetailTask, profileTask);

            var userDetail = userDetailTask.Result;
            var profile    = profileTask.Result;
            userDetail.AcceptedSubmissionCount = profile.AcceptedSubmissionCount;
            userDetail.TotalSubmissionCount    = profile.TotalSubmissionCount;
            userDetail.TotalProblemTriedCount  = profile.TotalProblemTriedCount;
            userDetail.AcceptedProblemCount    = profile.AcceptedProblemCount;

            return Ok(userDetail);
        }
    }
}