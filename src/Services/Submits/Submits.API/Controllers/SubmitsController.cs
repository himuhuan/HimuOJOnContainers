#region

using System.Security.Claims;
using HimuOJ.Common.WebApiComponents.Extensions;
using HimuOJ.Common.WebHostDefaults.Infrastructure;
using HimuOJ.Services.Submits.API.Application.Objects;
using HimuOJ.Services.Submits.API.Application.Queries;
using HimuOJ.Services.Submits.API.Services;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using HimuOJ.Services.Submits.Domain.Events;
using HimuOJ.Services.Submits.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace HimuOJ.Services.Submits.API.Controllers
{
    [Route("submissions")]
    [ApiController]
    public class SubmitsController : ControllerBase
    {
        private readonly IEventBusService _bus;
        private readonly ILogger<SubmitsController> _logger;
        private readonly IMediator _mediator;
        private readonly ISubmitsQuery _query;
        private readonly ISubmitsRepository _repository;

        public SubmitsController(
            ILogger<SubmitsController> logger,
            ISubmitsRepository repository,
            IMediator mediator,
            IEventBusService bus,
            ISubmitsQuery query)
        {
            _logger     = logger;
            _repository = repository;
            _mediator   = mediator;
            _bus        = bus;
            _query      = query;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType<ApiResult<CreateSubmitResponse>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Submit(CreateSubmitRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? throw new InvalidOperationException("User ID not found in claims.");

            var submission = new Submission(
                request.ProblemId, userId, request.SourceCode,
                DateTime.UtcNow, request.CompilerName);

            _repository.Add(submission);

            await _repository.UnitOfWork.SaveEntitiesAsync();
            await _mediator.Publish(new SubmissionCreatedDomainEvent(submission));

            return (new CreateSubmitResponse(submission.Id)).ToHttpApiResult(ApiResultCode.Ok);
        }

        [HttpPost("{id}/force-start")]
        [Authorize]
        public async Task<IActionResult> ForceStartJudgeSubmission(int id)
        {
            _logger.LogInformation("Force starting submission {SubmissionId}...", id);

            var submission = await _repository.GetAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            await _bus.PublishSubmissionReadyToJudgeAsync(submission.Id, submission.ProblemId);
            return Ok();
        }

        [HttpGet("statistics/problems-list/{problemId}")]
        public async Task<IActionResult> QueryStatistics(int problemId)
        {
            var statistics =
                await _query.GetProblemSubmitStatisticsAsync(problemId);
            return statistics.ToHttpApiResult();
        }

        [HttpGet("list")]
        public async Task<IActionResult> QuerySubmits([FromQuery] GetSubmissionsListRequest request)
        {
            var submits = await _query.GetSubmissionListAsync(request);
            return submits.ToHttpApiResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmission(int id)
        {
            var submission = await _query.GetSubmissionAsync(id);
            return submission == null ? NotFound() : submission.ToHttpApiResult(ApiResultCode.Ok);
        }

        /// <summary>
        /// Retrieves the submission statistics for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose statistics are to be retrieved.</param>
        /// <returns>An <see cref="IActionResult"/> containing the user's submission statistics.</returns>
        [HttpGet("statistics/user-profile/{userId}")]
        public async Task<IActionResult> GetStatisticsByUser(string userId)
        {
            var statistics = await _query.GetUserProfileStatisticsAsync(userId);
            return statistics.ToHttpApiResult();
        }
    }
}