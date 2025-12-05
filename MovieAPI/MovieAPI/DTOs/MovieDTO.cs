namespace MovieAPI.DTOs;

public class MovieDTO
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public string trailer { get; set; }
    public bool nowPlaying { get; set; }
    public DateTime releaseDate { get; set; }
    public string poster {get; set;}
    public List<GenresDTO>  genres { get; set; }
    public List<MovieActorDTO> actors { get; set; }
    public List<TheatherDTO> theather { get; set; }
}