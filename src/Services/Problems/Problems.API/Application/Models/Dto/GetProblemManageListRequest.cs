namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class GetProblemManageListRequest
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required Guid DistributorId { get; set; }
}