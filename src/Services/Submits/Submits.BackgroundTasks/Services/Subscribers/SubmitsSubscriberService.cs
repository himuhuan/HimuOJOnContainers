using DotNetCore.CAP;
using HimuOJ.Services.Submits.API.Application.IntegrationEvents;
using Submits.BackgroundTasks.Services.Judge;

namespace Submits.BackgroundTasks.Services.Subscribers;

public class SubmitsSubscriberService : ISubmitsSubscriberService, ICapSubscribe
{
    private readonly ILogger<SubmitsSubscriberService> _logger;
    private readonly IJudgeService _judgeService;

    public SubmitsSubscriberService(ILogger<SubmitsSubscriberService> logger, IJudgeService judgeService)
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