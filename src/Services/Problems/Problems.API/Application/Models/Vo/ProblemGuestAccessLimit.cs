namespace HimuOJ.Services.Problems.API.Application.Models.Vo;

public class ProblemGuestAccessLimit
{
    public required string ProblemTitle { get; set; }
    public required bool AllowDownloadInput { get; set; }
    public required bool AllowDownloadAnswer { get; set; }
}
