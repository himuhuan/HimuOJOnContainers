namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class GetProblemTitleListRequest
{
    public IEnumerable<int>? Ids { get; set; }
}