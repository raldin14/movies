using System.ComponentModel.DataAnnotations;

namespace MovieAPI.DTOs;

public class RatingDTO
{
    public int MovieId { get; set; }
    [Range(1,5)]
    public int Rate { get; set; }
}