using Submits.BackgroundTasks.Services.IntegrationEvents;

namespace HimuOJ.Services.Submits.API.Services;
public interface IEventBusService
{
    Task HandleSubmissionJudgeFinished(SubmissionJudgeFinishedIntegrationEvent @event);
    Task PublishSubmissionReadyToJudgeAsync(int submissionId, int? problemId);
}