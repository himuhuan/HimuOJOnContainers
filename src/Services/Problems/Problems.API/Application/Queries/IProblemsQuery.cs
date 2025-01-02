using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Queries;

public interface IProblemsQuery
{
    Task<ApiResult<Problem>> GetProblemAsync(int id);
    
    Task<ApiResult<ProblemList>> GetProblemListAsync(GetProblemsListRequest request);
    
    Task<ApiResult<string>> GetProblemTitleAsync(int id);

    Task<ApiResult<Dictionary<int, string>>> GetProblemTitlesAsync(GetProblemTitleListRequest request);
}