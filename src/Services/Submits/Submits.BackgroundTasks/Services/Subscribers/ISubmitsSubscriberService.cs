using HimuOJ.Services.Submits.API.Application.IntegrationEvents;

namespace Submits.BackgroundTasks.Services.Subscribers;

public interface ISubmitsSubscriberService
{
    void SubmissionReadyToJudge(SubmissionReadyToJudgeIntegrationEvent @event);
}