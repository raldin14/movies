using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MovieAPI.Entity;

public class Rating
{
    public int Id { get; set; }
    [Range(1,5)]
    public int rate { get; set; }
    public int movieId { get; set; }
    public Movie Movie { get; set; }
    public string userId { get; set; }
    public IdentityUser  User { get; set; }
}