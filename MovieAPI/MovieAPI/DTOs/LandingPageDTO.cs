namespace MovieAPI.DTOs;

public class LandingPageDTO
{
    public List<MovieDTO> NowPlaying  { get; set; }
    public List<MovieDTO> ComingUp {get; set;}
}