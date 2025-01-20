#region

using HimuOJ.Common.WebHostDefaults.Infrastructure.Event;

#endregion

namespace HimuOJ.Services.Submits.API.Application.IntegrationEvents;

public class SubmissionReadyToJudgeIntegrationEvent
    : IIntegrationEvent
{
    public const string EVENT_NAME = "himuoj.submits.submission.ready";

    public int ProblemId { get; set; }

    public int SubmissionId { get; set; }

    public string EventName => EVENT_NAME;
}