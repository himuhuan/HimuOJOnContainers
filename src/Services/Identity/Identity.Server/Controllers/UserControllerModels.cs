namespace Identity.Server.Controllers;

public class UserBrief
{
    public required string UserName { get; set; }
    public required string Avatar { get; set; }
}

public class GetUserBriefsRequest
{
    public required IEnumerable<string> Id { get; set; }
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
    public required string Email { get; set; }
}