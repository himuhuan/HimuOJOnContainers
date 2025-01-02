using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Web.WebSPA.Models;
using Refit;

namespace HimuOJ.Web.WebSPA.Services;

public interface IProblemsApi
{
    [Get("/api/problems/_list")]
    Task<BffProblemList> GetProblemsListAsync([Query] GetProblemsListRequest request);

    [Get("/api/problems/{id}/title")]
    Task<string> GetProblemTitleAsync(int id);

    [Get("/api/problems/titles")]
    Task<IDictionary<int, string>> GetProblemTitlesAsync([Query] GetProblemTitleListRequest request);
}