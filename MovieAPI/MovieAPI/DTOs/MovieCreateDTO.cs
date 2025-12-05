using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Utils;

namespace MovieAPI.DTOs;

public class MovieCreateDTO
{
    [Required]
    [StringLength(maximumLength: 300)]
    public string title { get; set; }
    public string description { get; set; }
    public string trailer { get; set; }
    public bool nowPlaying { get; set; }
    public DateTime releaseDate { get; set; }
    public IFormFile poster {get; set;}
    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> genreIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> theatherIds { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder<List<ActorMovieCreateDTO>>))]
    public List<ActorMovieCreateDTO> actors { get; set; }
}