namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class CreateTestPointRequest
{
    public string? Remarks { get; set; }
    public required string Input { get; set; }
    public required string ExpectedOutput { get; set; }
}