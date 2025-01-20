#region

using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Submits.Infrastructure;
using Microsoft.EntityFrameworkCore;

#endregion

namespace HimuOJ.Services.Submits.API.Application.Queries;

public class SubmitsQuery : ISubmitsQuery
{
    private readonly SubmitsDbContext _context;

    public SubmitsQuery(SubmitsDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResult<ProblemSubmitStatistics>> GetProblemSubmitStatisticsAsync(
        int problemId)
    {
        const string rawSql = """
                              SELECT COUNT(*) AS "TotalSubmits",
                                     COUNT(CASE WHEN "Status" = 'Accepted' THEN 1 END) AS "AcceptedSubmits"
                              FROM submits.t_submissions
                              WHERE "ProblemId" = @problemId
                              """;

        var statistics = await _context
            .Database
            .RawQueryAsync<ProblemSubmitStatistics>(rawSql, new { problemId })
            .FirstOrDefault();

        return statistics == null
            ? ApiResult<ProblemSubmitStatistics>.Success(new ProblemSubmitStatistics(0, 0))
            : ApiResult<ProblemSubmitStatistics>.Success(statistics);
    }

    public async Task<ApiResult<SubmissionList>> GetSubmissionListAsync(
        GetSubmissionsListRequest request)
    {
        var query = _context.Submissions
            .AsNoTracking()
            .OrderByDescending(s => s.SubmitTime)
            .WhereIf(request.ProblemId.HasValue, s => s.ProblemId == request.ProblemId)
            .WhereIf(request.SubmitterId != null, s => s.SubmitterId == request.SubmitterId)
            .Select(s => new SubmissionListItem
            {
                Id           = s.Id,
                ProblemId    = s.ProblemId,
                Usage        = s.Usage,
                Status       = s.Status,
                SubmitterId  = s.SubmitterId,
                SubmitTime   = s.SubmitTime,
                CompilerName = s.CompilerName
            });

        var total = await query.CountAsync();
        var items = await query.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return ApiResult<SubmissionList>.Success(new SubmissionList
        {
            Total     = total,
            PageCount = (total + request.PageSize - 1) / request.PageSize,
            Items     = items
        });
    }

    public async Task<GetSubmissionResult?> GetSubmissionAsync(int submissionId)
    {
        var submission = await _context.Submissions
            .AsNoTracking()
            .Where(s => s.Id == submissionId)
            .Include(s => s.TestPointResults)
            .SingleOrDefaultAsync();

        if (submission == null)
            return null;

        var result = new GetSubmissionResult
        {
            Id               = submission.Id,
            ProblemId        = submission.ProblemId,
            Usage            = submission.Usage,
            Status           = submission.Status,
            SubmitterId      = submission.SubmitterId,
            SubmitTime       = submission.SubmitTime,
            CompilerName     = submission.CompilerName,
            StatusMessage    = submission.StatusMessage,
            SourceCode       = submission.SourceCode,
            TestPointResults = [.. submission.TestPointResults]
        };

        Array.Sort(result.TestPointResults, (a, b) => a.TestPointId - b.TestPointId);
        return result;
    }

    public async Task<ApiResult<UserProfileStatistics>> GetUserProfileStatisticsAsync(string userId)
    {
        const string query
            = """
              SELECT COUNT(*) AS "TotalSubmissionCount",
              COUNT(CASE WHEN "Status" = 'Accepted' THEN 1 END) AS "AcceptedSubmissionCount",
              COUNT(DISTINCT "ProblemId") as "TotalProblemTriedCount",
              COUNT(DISTINCT case when "Status" = 'Accepted' then "ProblemId" end) as "AcceptedProblemCount"
              FROM submits.t_submissions
              WHERE "SubmitterId" = @submitterId;
              """;

        UserProfileStatistics result =
            await _context
                .Database
                .RawQueryAsync<UserProfileStatistics>(query, new { submitterId = userId })
                .FirstOrDefault() ?? new UserProfileStatistics();

        return result.ToApiResult(ApiResultCode.Ok);
    }
}