using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs;

public class ActorCreateDTO
{
    [Required]
    [StringLength(maximumLength:200)]
    public string name { get; set; }
    public string biography { get; set; }
    public DateTime dateOfBirth { get; set; }
    public IFormFile Picture { get; set; }
}