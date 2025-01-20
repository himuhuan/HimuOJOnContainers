#region

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HimuOJ.Services.Submits.Domain.AggregatesModel.SubmitAggregate;

#endregion

namespace HimuOJ.Services.Submits.API.Application.Queries;

public record ProblemSubmitStatistics(int TotalSubmits, int AcceptedSubmits)
{
    public ProblemSubmitStatistics()
        : this(0, 0)
    {
    }
}

// TODO: check for page and page size range
public class GetSubmissionsListRequest
{
    [Required]
    public int Page { get; set; }

    [Required]
    public int PageSize { get; set; }

    public int? ProblemId { get; set; }
    public string? SubmitterId { get; set; }
}

public class SubmissionListItem
{
    public required int Id { get; init; }

    public int? ProblemId { get; init; }

    public ResourceUsage? Usage { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JudgeStatus Status { get; init; }

    public string? SubmitterId { get; init; }

    public DateTime SubmitTime { get; init; }

    public required string CompilerName { get; init; }
}

public class SubmissionList
{
    public int Total { get; init; }
    public int PageCount { get; init; }
    public IEnumerable<SubmissionListItem> Items { get; init; } = [];
}

public class GetSubmissionResult
{
    public required int Id { get; init; }

    public required int? ProblemId { get; init; }

    public required ResourceUsage? Usage { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required JudgeStatus Status { get; init; }

    public required string? SubmitterId { get; init; }

    public required DateTime SubmitTime { get; init; }

    public required string CompilerName { get; init; }

    public required string StatusMessage { get; init; }

    public required string SourceCode { get; init; }

    public required TestPointResult[] TestPointResults { get; init; }
}

// GET /submissions/statistics/user-profile/{userId}
public class UserProfileStatistics
{
    public int TotalSubmissionCount { get; set; }
    public int AcceptedSubmissionCount { get; set; }
    public int TotalProblemTriedCount { get; set; }
    public int AcceptedProblemCount { get; set; }
}