namespace MovieAPI.DTOs;

public class MoviesPostGetDTO
{
    public List<GenresDTO>  Genres { get; set; }
    public List<TheatherDTO>  Theather { get; set; }
}