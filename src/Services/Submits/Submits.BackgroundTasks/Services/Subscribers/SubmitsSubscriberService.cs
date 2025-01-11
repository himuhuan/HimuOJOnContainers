#region

using DotNetCore.CAP;
using HimuOJ.Services.Submits.API.Application.IntegrationEvents;
using Submits.BackgroundTasks.Services.Judge;

#endregion

namespace Submits.BackgroundTasks.Services.Subscribers;

public class SubmitsSubscriberService : ISubmitsSubscriberService, ICapSubscribe
{
    private readonly IJudgeService _judgeService;
    private readonly ILogger<SubmitsSubscriberService> _logger;

    public SubmitsSubscriberService(
        ILogger<SubmitsSubscriberService> logger,
        IJudgeService judgeService)
    {
        _logger       = logger;
        _judgeService = judgeService;
    }

    [CapSubscribe(SubmissionReadyToJudgeIntegrationEvent.EVENT_NAME)]
    public void SubmissionReadyToJudge(SubmissionReadyToJudgeIntegrationEvent @event)
    {
        _logger.LogInformation("Received event {EventName} with SubmissionId={@EventId}",
            @event.EventName, @event.SubmissionId);
        _judgeService.AddJudgeTask(@event.SubmissionId);
    }
}