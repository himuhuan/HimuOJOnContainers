using HimuOJ.Services.Problems.Domain.AggregatesModel.ProblemAggregate;

namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class UpdateProblemRequest
{
    public required string Title { get;  set; }
    public required string Content { get;  set; }
    public required ResourceLimit DefaultResourceLimit { get;  set; }
    public required GuestAccessLimit GuestAccessLimit { get;  set; }
    public required IEnumerable<TestPoint> TestPoints { get;  set; }
}