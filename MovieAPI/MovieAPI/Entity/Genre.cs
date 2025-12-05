using System.ComponentModel.DataAnnotations;
using MovieAPI.Validations;

namespace MovieAPI.Entity;

public class Genre
{
    public int id { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(maximumLength:50)]
    [FirstLetterAsCapital]
    public string name { get; set; }
    public List<MoviesGenres> MoviesGenres { get; set; }
}