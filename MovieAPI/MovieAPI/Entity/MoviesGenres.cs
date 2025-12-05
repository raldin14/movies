namespace MovieAPI.Entity;

public class MoviesGenres
{
    public int genreId { get; set; }
    public int movieId { get; set; }
    public Movie movie { get; set; }
    public Genre genre { get; set; }
}