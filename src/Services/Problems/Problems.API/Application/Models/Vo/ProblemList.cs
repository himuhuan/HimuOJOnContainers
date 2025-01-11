namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

public class ProblemList
{
    public int Total { get; init; }
    public int PageCount { get; init; }
    public IEnumerable<ProblemListItem> Items { get; init; } = [];
}