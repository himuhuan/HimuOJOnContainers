namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

/// <summary>
/// Represents a list of problem management items.
/// </summary>
public class ProblemManageList
{
    /// <summary>
    /// Gets or sets the total number of problems.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Gets or sets the number of pages.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Gets or sets the collection of problem management list items.
    /// </summary>
    public required IEnumerable<ProblemManageListItem> Items { get; set; }
}