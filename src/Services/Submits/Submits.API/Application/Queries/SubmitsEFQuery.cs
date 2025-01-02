using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Submits.API.Application.Objects;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using HimuOJ.Services.Submits.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HimuOJ.Services.Submits.API.Application.Queries;

public class SubmitsEFQuery : ISubmitsQuery
{
    private readonly SubmitsDbContext _context;

    public SubmitsEFQuery(SubmitsDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResult<ProblemSubmitStatistics>> GetProblemSubmitStatisticsAsync(int problemId)
    {
        var statistics = await _context.Submissions
                                       .Where(s => s.ProblemId == problemId)
                                       .GroupBy(s => 1)
                                       .Select(g => new ProblemSubmitStatistics(
                                           g.Count(),
                                           g.Count(s => s.Status == JudgeStatus.Accepted)
                                       ))
                                       .FirstOrDefaultAsync();
        return statistics == null
            ? ApiResult<ProblemSubmitStatistics>.Success(new ProblemSubmitStatistics(0, 0))
            : ApiResult<ProblemSubmitStatistics>.Success(statistics);
    }

    public async Task<ApiResult<SubmissionList>> GetSubmissionListAsync(GetSubmissionsListRequest request)
    {
        var query = _context.Submissions
                            .AsNoTracking()
                            .OrderByDescending(s => s.SubmitTime)
                            .WhereIf(request.ProblemId.HasValue, s => s.ProblemId == request.ProblemId)
                            .WhereIf(request.SubmitterId != null, s => s.SubmitterId == request.SubmitterId)
                            .Select(s => new SubmissionListItem
                            {
                                Id = s.Id,
                                ProblemId = s.ProblemId,
                                Usage = s.Usage,
                                Status = s.Status,
                                SubmitterId = s.SubmitterId,
                                SubmitTime = s.SubmitTime,
                                CompilerName = s.CompilerName
                            });

        var total = await query.CountAsync();
        var items = await query.Skip((request.Page - 1) * request.PageSize)
                               .Take(request.PageSize)
                               .ToListAsync();

        return ApiResult<SubmissionList>.Success(new SubmissionList
        {
            Total = total,
            PageCount = (total + request.PageSize - 1) / request.PageSize,
            Items = items
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
            Id = submission.Id,
            ProblemId = submission.ProblemId,
            Usage = submission.Usage,
            Status = submission.Status,
            SubmitterId = submission.SubmitterId,
            SubmitTime = submission.SubmitTime,
            CompilerName = submission.CompilerName,
            StatusMessage = submission.StatusMessage,
            SourceCode = submission.SourceCode,
            TestPointResults = [.. submission.TestPointResults]
        };
        
        Array.Sort(result.TestPointResults, (a, b) => a.TestPointId - b.TestPointId);
        return result;
    }
}