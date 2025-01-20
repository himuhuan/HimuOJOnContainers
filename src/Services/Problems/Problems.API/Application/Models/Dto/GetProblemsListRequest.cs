using System.ComponentModel.DataAnnotations;


namespace HimuOJ.Services.Problems.API.Application.Models.Dto;

public class GetProblemsListRequest
{
    [Required]
    public int Page { get; set; }

    [Required]
    public int PageSize { get; set; }
}

