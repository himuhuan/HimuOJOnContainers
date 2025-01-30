#region

using HimuOJ.Services.Problems.API.Application.Models.Dto;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Web.WebSPA.Models;
using Refit;

#endregion

namespace HimuOJ.Web.WebSPA.Services;

public interface IProblemsApi
{
    [Get("/api/problems/_list")]
    Task<BffProblemList> GetProblemsListAsync([Query] GetProblemsListRequest request);

    [Get("/api/problems/titles")]
    Task<IDictionary<int, string>> GetProblemTitlesAsync(
        [Query] GetProblemTitleListRequest request);

    [Get("/api/problems/{id}/guest-access")]
    Task<ProblemGuestAccessLimit> GetProblemGuestAccessLimit(int id);
}