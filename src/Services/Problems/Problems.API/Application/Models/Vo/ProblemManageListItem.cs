using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

public class ProblemManageListItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime LastModifyTime { get; set; }
    public required ResourceLimit DefaultResourceLimit { get; set; }
    public required GuestAccessLimit GuestAccessLimit { get; set; }
}