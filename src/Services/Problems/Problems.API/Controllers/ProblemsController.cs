#region

using System.Security.Claims;
using HimuOJ.Common.BucketStorage;
using HimuOJ.Common.WebApiComponents.Authorization;
using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Problems.API.Application.Auth;
using HimuOJ.Services.Problems.API.Application.Models.Dto;
using HimuOJ.Services.Problems.API.Application.Models.Vo;
using HimuOJ.Services.Problems.API.Application.Queries;
using HimuOJ.Services.Problems.API.Application.Services;
using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;
using HimuOJ.Services.Problems.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace HimuOJ.Services.Problems.API.Controllers;

[Route("problems")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsQuery _query;
    private readonly IProblemsRepository _repository;
    private readonly IAuthorizationService _authorization;
    private readonly ILogger<ProblemsController> _logger;
    private readonly IResourceStorage _resource;

    public ProblemsController(
        IProblemsQuery query,
        IProblemsRepository repository,
        ILogger<ProblemsController> logger,
        IAuthorizationService authorization,
        IBucketStorage storage,
        IResourceStorage resource)
    {
        _query = query;
        _repository = repository;
        _logger = logger;
        _authorization = authorization;
        _resource = resource;
    }

    /// <summary>API /problems/{id}: Retrieves the full details of a problem by its ID.</summary>
    /// <param name="id">The ID of the problem to retrieve.</param>
    /// <returns>An <see cref="IActionResult" /> containing the full problem details.</returns>
    /// <response code="200">Returns the full problem details.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType<ProblemVo>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFullProblem(int id)
    {
        var result = await _query.GetProblemAsync(id);
        var authResult =
            await _authorization.AuthorizeAsync(User, result.Result, AuthorizationOperations.Read);
        return !authResult.Succeeded ? Forbid() : result.ToHttpApiResult();
    }

    [HttpGet("{id}/detail")]
    [ProducesResponseType<ApiResult<ProblemDetail>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemDetailAsync(int id)
    {
        var result = await _query.GetProblemDetailAsync(id);
        return result.ToHttpApiResult();
    }

    /// <summary>API /problems/{id}/title</summary>
    [HttpGet("{id}/title")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemTitleAsync(int id)
    {
        var result = await _query.GetProblemTitleAsync(id);
        return result.ToHttpApiResult();
    }

    /// <summary>API /problems/_list</summary>
    [HttpGet("_list")]
    [ProducesResponseType<ApiResult<ProblemList>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemsList([FromQuery] GetProblemsListRequest request)
    {
        var result = await _query.GetProblemListAsync(request);
        return result.ToHttpApiResult();
    }

    /// <summary>API /problems/titles</summary>
    [HttpGet("titles")]
    [ProducesResponseType<ApiResult<Dictionary<int, string>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemTitles(
        [FromQuery] GetProblemTitleListRequest request)
    {
        var result = await _query.GetProblemTitlesAsync(request);
        return result.ToHttpApiResult();
    }

    /// <summary>API /problems/management_list</summary>
    [HttpGet("management_list")]
    [Authorize]
    [ProducesResponseType<ProblemManageList>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProblemManageList(
        [FromQuery] GetProblemManageListRequest request)
    {
        var result = await _query.GetProblemManageListAsync(request);
        return result.ToHttpApiResult();
    }

    /// <summary>API /problems: Creates a new problem</summary>
    /// <param name="dto">The request containing the problem details.</param>
    /// <returns>The ID of the created problem.</returns>
    /// <response code="201">Returns the ID of the newly created problem.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    //[Authorize]
    [ProducesResponseType<int>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult>
        CreateProblemAsync([FromBody] ProblemDto dto)
    {
        string distributorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var problem = new Problem(
            new Guid(distributorId), dto.Title, dto.Content,
            new ResourceLimit(dto.MaxMemoryLimitByte, dto.MaxRealTimeLimitMilliseconds),
            new GuestAccessLimit(dto.AllowDownloadInput, dto.AllowDownloadAnswer));

        if (!(await _authorization.AuthorizeAsync(User, problem, AuthorizationOperations.Create))
            .Succeeded)
        {
            return Forbid();
        }

        foreach (var testPointDto in dto.TestPoints)
        {
            // TODO: add check for file existence
            problem.AddTestPoint(
                testPointDto.Input, testPointDto.ExpectedOutput,
                testPointDto.Remarks, TestPointResourceType.File);
        }

        _repository.Add(problem);
        await _repository.UnitOfWork.SaveEntitiesAsync();

        return CreatedAtAction(nameof(GetFullProblem), new { id = problem.Id }, problem.Id);
    }

    /// <summary>
    /// Uploads a resource file for a problem.
    /// </summary>
    /// <param name="id">The ID of the problem.</param>
    /// <param name="type">The type of the resource (input or answer).</param>
    /// <param name="file">The resource file to upload.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the upload operation.</returns>
    /// <response code="200">If the resource was successfully uploaded.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost("{id}/resources/{type}")]
    //[Authorize]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProblemResource(int id, string type, IFormFile file)
    {
        if (type != "input" && type != "answer")
            return ApiResultCode.BadRequest.ToHttpApiResult();

        if (!(await _query.IsProblemExistAsync(id)))
            return ApiResultCode.ResourceNotExist.ToHttpApiResult();

        bool isInput = type == "input";
        string? uploadedFileName;
        if (isInput)
            uploadedFileName = await _resource.UploadInputFileAsync(id, file);
        else
            uploadedFileName = await _resource.UploadExpectedOutputFileAsync(id, file);

        return uploadedFileName.ToHttpApiResult(ApiResultCode.Ok);
    }

    /// <summary>API PUT: /problems/{id}: Updates an existing problem with new details.</summary>
    /// <param name="id">The ID of the problem to update.</param>
    /// <param name="dto">The request containing the updated problem details.</param>
    /// <returns>An <see cref="IActionResult" /> indicating the result of the update operation.</returns>
    /// <response code="204">If the problem was successfully updated.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <remarks>
    ///     The method calculates which TestPoint entities remain unchanged, which need to be updated,
    ///     by comparing the old and new collections, via comparing their Ids. <br />
    ///     The method will <i>NOT</i> delete any TestPoint entities from the current collection. <br />
    ///     To add a new TestPoint, set ID of the TestPoint to 0, to 
    ///     remove a TestPoint, use DELETE /problems/{id}/testpoints instead.
    /// </remarks>
    [HttpPut("{id}")]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProblemAsync(int id, [FromBody] ProblemDto dto)
    {
        var problem = await _repository.GetAsync(id);
        if (problem == null)
        {
            return NotFound();
        }

        if (!(await _authorization.AuthorizeAsync(User, problem, AuthorizationOperations.Update))
            .Succeeded)
        {
            return Forbid();
        }

        bool success = problem.Update(dto.Title, dto.Content,
            dto.MaxMemoryLimitByte, dto.MaxRealTimeLimitMilliseconds,
            dto.AllowDownloadInput, dto.AllowDownloadAnswer,
            dto.TestPoints);

        if (!success)
            return BadRequest();

        await _repository.UnitOfWork.SaveEntitiesAsync();
        return NoContent();
    }

    /// <summary>
    /// API DELETE: /problems/{id}/testpoints
    /// </summary>
    /// <remarks>
    /// If the test point IDs are not found in the problem, they will be ignored.
    /// </remarks>
    /// <param name="id">The ID of the problem.</param>
    /// <param name="testPointIds">The IDs of the test points to delete.</param>
    /// <returns>An <see cref="IActionResult" /> indicating the result of the delete operation.</returns>
    /// <response code="204">If the test points were successfully deleted.</response>
    [Authorize]
    [HttpDelete("{id}/testpoints")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTestPointAsync(int id, [FromBody] int[] testPointIds)
    {
        _logger.LogInformation("Removing test points {@Ids} from problem {@ProblemId}",
            testPointIds,
            id);

        var problem = await _repository.GetProblemMinimalAsync(id);
        if (problem == null)
        {
            return NotFound();
        }

        if (!(await _authorization.AuthorizeAsync(User, problem, AuthorizationOperations.Update))
            .Succeeded)
        {
            return Forbid();
        }

        await _repository.RemoveTestPoints(id, testPointIds);
        return NoContent();
    }

    /// <summary>
    /// API DELETE: /problems/{id}
    /// </summary>
    /// <param name="id">The ID of the problem to delete.</param>
    /// <returns>An <see cref="IActionResult" /> indicating the result of the delete operation.</returns>
    /// <response code="204">If the problem was successfully deleted.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProblemAsync(int id)
    {
        var problem = await _repository.GetProblemMinimalAsync(id);
        if (problem == null)
        {
            return NotFound();
        }

        if (!(await _authorization.AuthorizeAsync(User, problem, AuthorizationOperations.Delete))
            .Succeeded)
        {
            return Forbid();
        }

        var success = await _repository.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        await _repository.UnitOfWork.SaveEntitiesAsync();
        return NoContent();
    }

    /// <summary>
    /// Downloads a resource file for a problem.
    /// </summary>
    /// <param name="id">The ID of the problem.</param>
    /// <param name="resourceName">The name of the resource file to download.</param>
    /// <returns>An <see cref="IActionResult"/> containing the resource file stream.</returns>
    /// <response code="200">Returns the resource file stream.</response>
    /// <response code="404">If the problem with the specified ID was not found.</response>
    /// <response code="403">If the user is not authorized to access the resource.</response>
    /// <response code="400">If the resource name is invalid.</response>
    [HttpGet("{id}/resources/{resourceName}")]
    [Authorize]
    public async Task<IActionResult> DownloadResource(int id, string resourceName)
    {
        var problem = await _repository.GetProblemMinimalAsync(id);
        if (problem == null)
        {
            return ApiResultCode.ResourceNotExist.ToHttpApiResult();
        }

        string resourceExtension = Path.GetExtension(resourceName);
        if (resourceExtension != ".in" && resourceExtension != ".out")
            return ApiResultCode.BadRequest.ToHttpApiResult("Bad resource name");
        var operationType = resourceExtension switch
        {
            ".in" => ProblemAuthorizationOperations.ReadInput,
            _ => ProblemAuthorizationOperations.ReadExpectedOutput
        };
        if (!(await _authorization.AuthorizeAsync(User, problem, operationType))
            .Succeeded)
        {
            return Forbid();
        }

        try
        {
            var stream = await _resource.DownloadResourceAsync(id, resourceName);
            return File(stream, "application/octet-stream", resourceName);
        }
        catch (FileNotFoundException e)
        {
            _logger.LogError(e, """Failed to download resource "{ResourceName}" for problem {ProblemId}""", resourceName, id);
            return ApiResultCode.ResourceNotExist.ToHttpApiResult();
        }
    }
}