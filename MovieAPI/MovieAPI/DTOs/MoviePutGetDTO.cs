namespace MovieAPI.DTOs;

public class MoviePutGetDTO
{
    public MovieDTO Movie { get; set; }
    public List<GenresDTO> SelectedGenres { get; set; }
    public List<GenresDTO> NoSelectedGenres { get; set; }
    public List<TheatherDTO> SelectedTheathers { get; set; }
    public List<TheatherDTO> NoSelectedTheathers { get; set; }
    public List<MovieActorDTO> Actors { get; set; }
}