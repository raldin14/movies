using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs;

public class TheatherCreateDTO
{
    [Required]
    [StringLength(maximumLength:75)]
    public string name { get; set; }
    [Range(-90, 90)]
    public double latitude { get; set; }
    [Range(-180, 180)]
    public double longitude { get; set; }
}