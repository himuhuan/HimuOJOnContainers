using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using Microsoft.AspNetCore.Mvc;

namespace HimuOJ.Services.Problems.API.Controllers;

[Route("problems")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsQuery _query;

    public ProblemsController(IProblemsQuery query)
    {
        _query = query;
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ApiResult<Problem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemAsync(int id)
    {
        var result = await _query.GetProblemAsync(id);
        return result.ToHttpApiResult();
    }
    
    [HttpGet("{id}/title")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemTitleAsync(int id)
    {
        var result = await _query.GetProblemTitleAsync(id);
        return result.ToHttpApiResult();
    }
    
    [HttpGet("_list")]
    [ProducesResponseType<ApiResult<ProblemList>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemsList([FromQuery] GetProblemsListRequest request)
    {
        var result = await _query.GetProblemListAsync(request);
        return result.ToHttpApiResult();
    }

    [HttpGet("titles")]
    [ProducesResponseType<ApiResult<Dictionary<int, string>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemTitles([FromQuery] GetProblemTitleListRequest request)
    {
        var result = await _query.GetProblemTitlesAsync(request);
        return result.ToHttpApiResult();
    }

}