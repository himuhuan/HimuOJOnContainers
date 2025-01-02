using HimuOJ.Common.WebHostDefaults.Infrastructure.Event;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

namespace Submits.BackgroundTasks.Services.IntegrationEvents;

public class SubmissionJudgeFinishedIntegrationEvent
    : IIntegrationEvent
{
    public const string EVENT_NAME = "himuoj.submits.submission.finished";
    public string EventName => EVENT_NAME;

    public int SubmissionId { get; set; }

    public JudgeStatus Status {  get; set; }

    public ResourceUsage? Usage { get; set; }
}
