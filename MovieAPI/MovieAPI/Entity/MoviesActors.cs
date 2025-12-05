using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Entity;

public class MoviesActors
{
    public int movieId { get; set; }
    public int actorId { get; set; }
    public Movie movie { get; set; }
    public Actor actor { get; set; }
    [StringLength(maximumLength:100)]
    public string character { get; set; }
    public int order { get; set; }
}