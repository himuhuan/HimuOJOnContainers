#region

using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.API.Application.Models.Dto;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

#endregion

namespace HimuOJ.Services.Problems.API.Application.Queries;

public interface IProblemsQuery
{
    Task<ApiResult<ProblemVo>> GetProblemAsync(int id);
    
    Task<ApiResult<ProblemDetail>> GetProblemDetailAsync(int id);

    Task<ApiResult<ProblemList>> GetProblemListAsync(GetProblemsListRequest request);

    Task<ApiResult<string>> GetProblemTitleAsync(int id);

    Task<ApiResult<Dictionary<int, string>>> GetProblemTitlesAsync(
        GetProblemTitleListRequest request);

    Task<ApiResult<ProblemManageList>>
        GetProblemManageListAsync(GetProblemManageListRequest request);
}