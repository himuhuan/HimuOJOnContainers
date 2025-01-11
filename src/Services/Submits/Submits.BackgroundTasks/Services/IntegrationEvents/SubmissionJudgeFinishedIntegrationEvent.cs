#region

using HimuOJ.Common.WebHostDefaults.Infrastructure.Event;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

#endregion

namespace Submits.BackgroundTasks.Services.IntegrationEvents;

public class SubmissionJudgeFinishedIntegrationEvent
    : IIntegrationEvent
{
    public const string EVENT_NAME = "himuoj.submits.submission.finished";

    public int SubmissionId { get; set; }

    public JudgeStatus Status { get; set; }

    public ResourceUsage? Usage { get; set; }
    public string EventName => EVENT_NAME;
}