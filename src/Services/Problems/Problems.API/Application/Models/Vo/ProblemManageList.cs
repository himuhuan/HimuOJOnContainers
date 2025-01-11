namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

public class ProblemManageList
{
    public int Total { get; set; }
    public int PageCount { get; set; }
    public required IEnumerable<ProblemManageListItem> Items { get; set; }
}