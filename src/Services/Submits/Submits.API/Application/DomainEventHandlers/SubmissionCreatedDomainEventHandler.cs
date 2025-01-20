#region

using DotNetCore.CAP;
using HimuOJ.Services.Submits.API.Services;
using HimuOJ.Services.Submits.Domain.Events;
using MediatR;

#endregion

namespace HimuOJ.Services.Submits.API.Application.DomainEventHandlers;

public class
    SubmissionCreatedDomainEventHandler : INotificationHandler<SubmissionCreatedDomainEvent>
{
    private readonly IEventBusService _eventBus;

    public SubmissionCreatedDomainEventHandler(
        ICapPublisher bus,
        ILogger<SubmissionCreatedDomainEventHandler> logger,
        IEventBusService eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task Handle(
        SubmissionCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await _eventBus.PublishSubmissionReadyToJudgeAsync(
            notification.Submission.Id,
            notification.Submission.ProblemId);
    }
}