namespace MovieAPI.DTOs;

public class RequestAuthentication
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}