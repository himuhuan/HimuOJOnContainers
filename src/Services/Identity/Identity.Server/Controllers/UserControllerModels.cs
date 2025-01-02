namespace Identity.Server.Controllers;

public class UserBrief
{
    // public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Avatar { get; set; }
}

public class GetUserBriefsRequest
{
    public required IEnumerable<string> Id { get; set; }
}