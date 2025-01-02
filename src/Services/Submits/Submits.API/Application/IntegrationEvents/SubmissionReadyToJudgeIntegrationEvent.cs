using HimuOJ.Common.WebHostDefaults.Infrastructure.Event;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

namespace HimuOJ.Services.Submits.API.Application.IntegrationEvents;

public class SubmissionReadyToJudgeIntegrationEvent
    : IIntegrationEvent
{
    public const string EVENT_NAME = "himuoj.submits.submission.ready";

    public string EventName => EVENT_NAME;

    public int ProblemId { get; set; }

    public int SubmissionId { get; set; }
}