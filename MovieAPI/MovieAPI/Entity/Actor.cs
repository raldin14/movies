using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Entity;

public class Actor
{
    public int id { get; set; }
    [Required]
    [StringLength(maximumLength:200)]
    public string name { get; set; }
    public string biography { get; set; }
    public DateTime dateOfBirth { get; set; }
    public string picture { get; set; } = "";
    public List<MoviesActors> MoviesActors { get; set; }
}