using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs;

public class UserCredencials
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}