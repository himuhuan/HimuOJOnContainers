#region

using System.Globalization;
using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.API.Application.Models.Dto;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure;
using Microsoft.EntityFrameworkCore;

#endregion

namespace HimuOJ.Services.Problems.API.Application.Queries;

public class ProblemsQuery : IProblemsQuery
{
    private readonly ProblemsDbContext _context;

    public ProblemsQuery(ProblemsDbContext context, ILogger<ProblemsQuery> logger)
    {
        _context = context;
    }

    public async Task<ApiResult<ProblemVo>> GetProblemAsync(int id)
    {
        var problem = await _context.Problems
            .AsNoTracking()
            .Include(p => p.TestPoints)
            .Where(p => p.Id == id)
            .SingleOrDefaultAsync();

        if (problem == null)
            return ApiResult<ProblemVo>.Error(ApiResultCode.ResourceNotExist);

        var vo = new ProblemVo
        {
            Id                  = problem.Id,
            Title               = problem.Title,
            Content             = problem.Content,
            AllowDownloadAnswer = problem.GuestAccessLimit.AllowDownloadInput,
            AllowDownloadInput  = problem.GuestAccessLimit.AllowDownloadInput,
            TestPoints          = problem.TestPoints.ToList(),
            
            CreateTime     = problem.CreateTime.ToString(CultureInfo.InvariantCulture),
            LastModifyTime = problem.LastModifyTime.ToString(CultureInfo.InvariantCulture),

            MaxMemoryLimitByte = problem.DefaultResourceLimit.MaxMemoryLimitByte,
            MaxRealTimeLimitMilliseconds =
                problem.DefaultResourceLimit.MaxRealTimeLimitMilliseconds,
        };

        return ApiResult<ProblemVo>.Success(vo);
    }

    public async Task<ApiResult<ProblemDetail>> GetProblemDetailAsync(int id)
    {
        var detail = await _context.Problems
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProblemDetail
            {
                Content              = p.Content,
                CreateTime           = p.CreateTime.ToString(CultureInfo.InvariantCulture),
                DefaultResourceLimit = p.DefaultResourceLimit,
                Title                = p.Title
            })
            .SingleOrDefaultAsync();
        return detail == null
            ? ApiResult<ProblemDetail>.Error(ApiResultCode.ResourceNotExist)
            : detail.ToApiResult(ApiResultCode.Ok);
    }

    public async Task<ApiResult<ProblemList>> GetProblemListAsync(GetProblemsListRequest request)
    {
        var query = _context.Problems.AsNoTracking();

        var total = await GetProblemsCountAsync();
        var list = await query.Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .Select(p => new ProblemListItem
            {
                Id    = p.Id,
                Title = p.Title
            })
            .ToListAsync();

        return ApiResult<ProblemList>.Success(new ProblemList
        {
            Total     = total,
            PageCount = total / request.PageSize + (total % request.PageSize == 0 ? 0 : 1),
            Items     = list
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
        var idList = request.Ids?.ToList() ?? [];

        // NOTE: If the number of Ids in the request is huge,
        // the following query performance may be poor.
        Dictionary<int, string> titles = await _context.Problems.AsNoTracking()
            .Where(p => idList.Contains(p.Id))
            .Select(p => new { p.Id, p.Title })
            .ToDictionaryAsync(p => p.Id, p => p.Title);

        return titles.ToApiResult(ApiResultCode.Ok);
    }

    public async Task<int> GetProblemsCountAsync()
    {
        return await _context.Problems.CountAsync();
    }

    // Get /problems/management_list
    public async Task<ApiResult<ProblemManageList>>
        GetProblemManageListAsync(GetProblemManageListRequest request)
    {
        var query = _context.Problems.AsNoTracking();

        var total = await GetProblemsCountAsync();
        var list = await query
            .Where(p => p.DistributorId == request.DistributorId)
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .OrderByDescending(p => p.Id)
            .Select(p => new ProblemManageListItem
            {
                Id                   = p.Id,
                Title                = p.Title,
                CreateTime           = p.CreateTime,
                LastModifyTime       = p.LastModifyTime,
                DefaultResourceLimit = p.DefaultResourceLimit,
                GuestAccessLimit     = p.GuestAccessLimit
            })
            .ToListAsync();

        return ApiResult<ProblemManageList>.Success(new ProblemManageList
        {
            Total     = total,
            PageCount = total / request.PageSize + (total % request.PageSize == 0 ? 0 : 1),
            Items     = list
        });
    }
}