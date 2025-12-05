namespace MovieAPI.Entity;

public class MoviesTheathers
{
    public int movieId { get; set; }
    public int theaterId { get; set; }
    public Movie movie { get; set; }
    public Theather theater { get; set; }
}