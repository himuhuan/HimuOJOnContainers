using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

/// <summary>
/// Problem data transfer object.
/// </summary>
public class ProblemDto
{
    /// <summary>
    /// the title of the problem.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// the content of the problem.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// the maximum memory limit in bytes.
    /// </summary>
    public required long MaxMemoryLimitByte { get; set; }

    /// <summary>
    /// the maximum real-time limit in milliseconds.
    /// </summary>
    public required int MaxRealTimeLimitMilliseconds { get; set; }

    /// <summary>
    /// Whether downloading input is allowed.
    /// </summary>
    public required bool AllowDownloadInput { get; set; }

    /// <summary>
    /// Whether downloading the answer is allowed.
    /// </summary>
    public required bool AllowDownloadAnswer { get; set; }
    
    /// <summary>
    /// Test points of the problem.
    /// </summary>
    public required IEnumerable<TestPoint> TestPoints { get; set; }
}