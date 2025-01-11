#region

using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;
using Microsoft.AspNetCore.SignalR;

#endregion

namespace HimuOJ.Services.Submits.API.Hubs;

public class SubmissionStatusHub : Hub
{
    // Currently, only the server can actively send messages to clients
    // so there is no need to implement client methods
}

public static class SubmissionStatusHubMethods
{
    public static async Task SendSubmissionStatusAsync(
        this IHubContext<SubmissionStatusHub> hubContext,
        int submissionId,
        string status,
        ResourceUsage? usage = null)
    {
        await hubContext.Clients.All
            .SendAsync("ReceiveSubmissionStatus", submissionId, status, usage);
    }
}