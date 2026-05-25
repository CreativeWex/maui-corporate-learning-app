namespace LmsApp.Models.Domain;

public class AuthToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
}
