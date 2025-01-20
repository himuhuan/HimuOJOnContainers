namespace Identity.Server.Controllers;

public class UserBrief
{
    public required string UserName { get; set; }
    public required string Avatar { get; set; }
}

public class GetUserBriefsRequest
{
    public IEnumerable<string>? Ids { get; set; }
}

public class UserDetail
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string RegisterDate { get; set; }
    public required string LastLoginDate { get; set; }
}

public class CreateUserRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string RepeatedPassword { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
}