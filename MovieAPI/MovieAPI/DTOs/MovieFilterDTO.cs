namespace MovieAPI.DTOs;

public class MovieFilterDTO
{
    public int page  { get; set; }
    public int pageSize { get; set; }
    public PaginationDTO pagination { get { return new PaginationDTO() {page = page, pageSize = pageSize}; } }
    public string ?  title { get; set; }
    public int genreId { get; set; }
    public bool nowPlaying { get; set; }
    public bool comingUp { get; set; }
}