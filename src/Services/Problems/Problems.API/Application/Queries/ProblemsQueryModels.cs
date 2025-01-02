using System.ComponentModel.DataAnnotations;

namespace HimuOJ.Services.Problems.API.Application.Queries;

public class GetProblemsListRequest
{
    [Required]
    public int Page { get; set; }

    [Required]
    public int PageSize { get; set; }
}

public class ProblemListItem
{
    public int Id { get; init; }
    public required string Title { get; init; }
}

public class ProblemList
{
    public int Total { get; init; }
    public int PageCount { get; init; }
    public IEnumerable<ProblemListItem> Items { get; init; } = [];
}

public class GetProblemTitleListRequest
{
    public required IEnumerable<int> Ids { get; set; }
}

public class ProblemTestPoint
{
    public required string Input { get; init; }
    public required string Output { get; init; }
}
