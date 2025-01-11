#region

using Submits.BackgroundTasks.Services.IntegrationEvents;

#endregion

namespace HimuOJ.Services.Submits.API.Services;

public interface IEventBusService
{
    Task HandleSubmissionJudgeFinished(SubmissionJudgeFinishedIntegrationEvent @event);
    Task PublishSubmissionReadyToJudgeAsync(int submissionId, int? problemId);
}