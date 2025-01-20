using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

/// <summary>
/// Represents the detailed information about a problem.
/// </summary>
public class ProblemDetail
{
    /// <summary>
    /// The title of the problem.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The content of the problem.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// The time when the problem was created.
    /// </summary>
    public required string CreateTime { get; set; }

    /// <summary>
    /// The default resource limit for the problem.
    /// </summary>
    public required ResourceLimit DefaultResourceLimit { get; set; }
}