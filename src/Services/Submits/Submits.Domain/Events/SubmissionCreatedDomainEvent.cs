#region

using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using MediatR;

#endregion

namespace HimuOJ.Services.Submits.Domain.Events;

public record SubmissionCreatedDomainEvent(Submission Submission)
    : INotification;