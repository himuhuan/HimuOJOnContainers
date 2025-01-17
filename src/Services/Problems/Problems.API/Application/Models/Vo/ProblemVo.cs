using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

/// <summary>
/// Represents a problem value object.
/// </summary>
public class ProblemVo
{
    /// <summary>
    /// problem ID.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// problem title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// problem content.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// maximum memory limit in bytes.
    /// </summary>
    public required long MaxMemoryLimitByte { get; set; }

    /// <summary>
    /// maximum real-time limit in milliseconds.
    /// </summary>
    public required long MaxRealTimeLimitMilliseconds { get; set; }

    /// <summary>
    ///  a value indicating whether downloading input is allowed.
    /// </summary>
    public required bool AllowDownloadInput { get; set; }

    /// <summary>
    /// a value indicating whether downloading answer is allowed.
    /// </summary>
    public required bool AllowDownloadAnswer { get; set; }

    /// <summary>
    /// creation time of problem.
    /// </summary>
    public required string CreateTime { get; set; }

    /// <summary>
    /// last modification time of problem.
    /// </summary>
    public required string LastModifyTime { get; set; }

    /// <summary>
    /// list of test points for problem.
    /// </summary>
    public required List<TestPoint> TestPoints { get; set; }
}