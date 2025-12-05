using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Entity;

public class Movie
{
    public int id { get; set; }
    [Required]
    [StringLength(maximumLength: 300)]
    public string title { get; set; }
    public string description { get; set; }
    public string trailer { get; set; }
    public bool nowPlaying { get; set; }
    public DateTime releaseDate { get; set; }
    public string poster {get; set;}
    public List<MoviesActors> MoviesActors { get; set; }
    public List<MoviesGenres> MoviesGenres { get; set; }
    public List<MoviesTheathers> MoviesTheathers { get; set; }
}