using DotNetCore.CAP;

using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Submits.API.Application.IntegrationEvents;
using HimuOJ.Services.Submits.API.Hubs;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using Microsoft.AspNetCore.SignalR;

using Submits.BackgroundTasks.Services.IntegrationEvents;

namespace HimuOJ.Services.Submits.API.Services;

public class EventBusService : ICapSubscribe, IEventBusService
{
    private readonly ILogger<EventBusService> _logger;
    private readonly IHubContext<SubmissionStatusHub> _hubContext;
    private readonly ICapPublisher _bus;

    public EventBusService(
        ILogger<EventBusService> logger,
        IHubContext<SubmissionStatusHub> hubContext,
        ICapPublisher bus)
    {
        _logger = logger;
        _hubContext = hubContext;
        _bus = bus;
    }

    public async Task PublishSubmissionReadyToJudgeAsync(int submissionId, int? problemId)
    {
        // TODO: for debug. Remove this line when the task pool is ready.
        // await Task.Delay(3000);
        await _hubContext.SendSubmissionStatusAsync(submissionId, JudgeStatus.Running.ToString());
        await _bus.PublishEventAsync(new SubmissionReadyToJudgeIntegrationEvent
        {
            SubmissionId = submissionId,
            ProblemId = problemId.GetValueOrDefault(0)
        });
    }

    #region Subscriber

    [CapSubscribe(SubmissionJudgeFinishedIntegrationEvent.EVENT_NAME)]
    public async Task HandleSubmissionJudgeFinished(SubmissionJudgeFinishedIntegrationEvent @event)
    {
        _logger.LogInformation("Submission {SubmissionId} judge finished with status {Status}.",
            @event.SubmissionId, @event.Status);
        await _hubContext.SendSubmissionStatusAsync(@event.SubmissionId, 
            @event.Status.ToString(), 
            @event.Usage);
    }

    #endregion
}
