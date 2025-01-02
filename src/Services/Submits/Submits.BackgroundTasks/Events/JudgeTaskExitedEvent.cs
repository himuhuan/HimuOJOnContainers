using DotNetCore.CAP;

using HimuOJ.Common.WebHostDefaults.Extensions;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using HimuOJ.Services.Submits.Infrastructure;
using MediatR;

using Submits.BackgroundTasks.Services.IntegrationEvents;

namespace Submits.BackgroundTasks.Events;

public record JudgeTaskExitedEvent(
    Submission Submission, int TestPointId, JudgeStatus Status,
    string? Message = null, OutputDifference? OutputDifference = null)
    : INotification
{
    public JudgeTaskExitedEvent(Submission submission, JudgeStatus status, string? message)
        : this(submission, -1, status, message)
    {
    }
}

public class JudgeTaskTestPointExitedEventHandler : INotificationHandler<JudgeTaskExitedEvent>
{
    private readonly ILogger<JudgeTaskTestPointExitedEventHandler> _logger;
    private readonly SubmitsDbContext _context;
    private readonly ICapPublisher _bus;

    public JudgeTaskTestPointExitedEventHandler(
        ILogger<JudgeTaskTestPointExitedEventHandler> logger,
        SubmitsDbContext context,
        ICapPublisher bus)
    {
        _logger = logger;
        _context = context;
        _bus = bus;
    }

    public async Task Handle(JudgeTaskExitedEvent notification, CancellationToken cancellationToken)
    {
        var submission = notification.Submission;
        var testPointId = notification.TestPointId;
        var status = notification.Status;
        var message = notification.Message;

        _logger.LogInformation(
            "Submission {SubmissionId} test point terminated with status {Status}: {Message}",
            submission.Id, status, message);

        if (testPointId != -1) { 
            submission.UpdateStatus(testPointId, status, message);
            if (status == JudgeStatus.WrongAnswer)
                submission.UpdateStatus(testPointId, notification.OutputDifference);
        } else
            submission.UpdateStatus(status, message);

        _context.Submissions.Update(submission);
        await _context.SaveEntitiesAsync(cancellationToken);

        await _bus.PublishEventAsync(new SubmissionJudgeFinishedIntegrationEvent
        {
            SubmissionId = submission.Id,
            Status = submission.Status,
            Usage = submission.Usage
        });
    }
}