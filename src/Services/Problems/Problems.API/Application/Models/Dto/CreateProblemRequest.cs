namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

/// <summary>
/// Represents a request to create a problem.
/// </summary>
public class CreateProblemRequest
{
    /// <summary>
    /// Gets or sets the title of the problem.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the problem.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the maximum memory limit in bytes.
    /// </summary>
    public required long MaxMemoryLimitByte { get; set; }

    /// <summary>
    /// Gets or sets the maximum real-time limit in milliseconds.
    /// </summary>
    public required int MaxRealTimeLimitMilliseconds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether downloading input is allowed.
    /// </summary>
    public required bool AllowDownloadInput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether downloading the answer is allowed.
    /// </summary>
    public required bool AllowDownloadAnswer { get; set; }
}