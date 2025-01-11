#region

using System.Security.Claims;
using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.API.Application.Models.Dto;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure;
using HimuOJ.Services.Problems.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace HimuOJ.Services.Problems.API.Controllers;

[Route("problems")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly ProblemsDbContext _context;
    private readonly IProblemsQuery _query;
    private readonly IProblemsRepository _repository;

    public ProblemsController(
        IProblemsQuery query,
        IProblemsRepository repository,
        ProblemsDbContext context)
    {
        _query      = query;
        _repository = repository;
        _context    = context;
    }

    /// <summary>
    ///     Retrieves the full details of a problem by its ID.
    /// </summary>
    /// <remarks>
    ///     Only authorized users can access this API.
    /// </remarks>
    /// <param name="id">The ID of the problem to retrieve.</param>
    /// <returns>An <see cref="IActionResult" /> containing the full problem details.</returns>
    /// <response code="200">Returns the full problem details.</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType<ApiResult<Problem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFullProblem(int id)
    {
        // TODO: add authorization check
        var result = await _repository.GetAsync(id);
        return result == null ? NotFound() : result.ToHttpApiResult(ApiResultCode.Ok);
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
    public async Task<IActionResult> GetProblemTitles(
        [FromQuery] GetProblemTitleListRequest request)
    {
        var result = await _query.GetProblemTitlesAsync(request);
        return result.ToHttpApiResult();
    }

    [HttpGet("management_list")]
    [Authorize]
    [ProducesResponseType<ProblemManageList>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemManageList(
        [FromQuery] GetProblemManageListRequest request)
    {
        var result = await _query.GetProblemManageListAsync(request);
        return result.ToHttpApiResult();
    }

    /// <summary>
    ///     Creates a new problem.
    /// </summary>
    /// <param name="request">The request containing the problem details.</param>
    /// <returns>The ID of the created problem.</returns>
    /// <response code="201">Returns the ID of the newly created problem.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType<int>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult>
        CreateProblemAsync([FromBody] CreateProblemRequest request)
    {
        string distributorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var problem = new Problem(
            new Guid(distributorId), request.Title, request.Content,
            new ResourceLimit(request.MaxMemoryLimitByte, request.MaxRealTimeLimitMilliseconds),
            new GuestAccessLimit(request.AllowDownloadInput, request.AllowDownloadAnswer));

        _repository.Add(problem);
        await _repository.UnitOfWork.SaveEntitiesAsync();

        return CreatedAtAction(nameof(GetFullProblem), new { id = problem.Id }, problem.Id);
    }

    /// <summary>
    ///     Updates an existing problem with new details.
    /// </summary>
    /// <param name="id">The ID of the problem to update.</param>
    /// <param name="request">The request containing the updated problem details.</param>
    /// <returns>
    ///     An <see cref="IActionResult" /> indicating the result of the update operation.
    /// </returns>
    /// <response code="204">If the problem was successfully updated.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    /// <remarks>
    ///     <para>
    ///         The method calculates which TestPoint entities remain unchanged, which need to be updated,
    ///         and which need to be deleted by comparing the old and new collections, via comparing their Ids.
    ///     </para>
    ///     <para>
    ///         Update: Existing TestPoint entities that exist in both the current
    ///         and updated collections are updated with new data.
    ///     </para>
    ///     <para>
    ///         Deletion: TestPoint entities that exist in the current collection
    ///         but not in the updated collection are removed.
    ///     </para>
    ///     <para>
    ///         Addition: New TestPoint entities from the updated collection are added to the current collection.
    ///     </para>
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProblemAsync(
        int id,
        [FromBody]
        UpdateProblemRequest request)
    {
        var problem = await _repository.GetAsync(id);
        if (problem == null)
        {
            return NotFound();
        }

        problem.Update(request.Title, request.Content,
            request.DefaultResourceLimit, request.GuestAccessLimit,
            request.TestPoints);

        await _repository.UnitOfWork.SaveEntitiesAsync();
        return NoContent();
    }
}