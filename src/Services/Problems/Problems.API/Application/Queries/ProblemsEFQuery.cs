using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HimuOJ.Services.Problems.API.Application.Queries;

public class ProblemsEFQuery : IProblemsQuery
{
    private readonly ProblemsDbContext _context;

    public ProblemsEFQuery(ProblemsDbContext context, ILogger<ProblemsEFQuery> logger)
    {
        _context = context;
    }

    public async Task<ApiResult<Problem>> GetProblemAsync(int id)
    {
        var problem = await _context.Problems.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return problem == null
            ? ApiResult<Problem>.Error(ApiResultCode.ResourceNotExist)
            : ApiResult<Problem>.Success(problem);
    }

    public async Task<int> GetProblemsCountAsync()
    {
        return await _context.Problems.CountAsync();
    }

    public async Task<ApiResult<ProblemList>> GetProblemListAsync(GetProblemsListRequest request)
    {
        var query = _context.Problems.AsNoTracking();

        var total = await GetProblemsCountAsync();
        var list = await query.Skip(request.PageSize * (request.Page - 1))
                              .Take(request.PageSize)
                              .Select(p => new ProblemListItem
                              {
                                  Id = p.Id,
                                  Title = p.Title
                              })
                              .ToListAsync();

        return ApiResult<ProblemList>.Success(new ProblemList
        {
            Total = total,
            PageCount = total / request.PageSize + (total % request.PageSize == 0 ? 0 : 1),
            Items = list
        });
    }

    public Task<ApiResult<string>> GetProblemTitleAsync(int id)
    {
        return _context.Problems.AsNoTracking()
                       .Where(p => p.Id == id)
                       .Select(p => p.Title)
                       .FirstOrDefaultAsync()
                       .ContinueWith(task => task.Result == null
                           ? ApiResult<string>.Error(ApiResultCode.ResourceNotExist)
                           : ApiResult<string>.Success(task.Result));
    }

    public async Task<ApiResult<Dictionary<int, string>>>
        GetProblemTitlesAsync(GetProblemTitleListRequest request)
    {
        var idList = request.Ids.ToList();

        // NOTE: If the number of Ids in the request is huge, the following query performance may be poor.
        Dictionary<int, string> titles = await _context.Problems.AsNoTracking()
            .Where(p => idList.Contains(p.Id))
            .Select(p => new { p.Id, p.Title })
            .ToDictionaryAsync(p => p.Id, p => p.Title);

        return titles.ToApiResult(ApiResultCode.Ok);
    }
}