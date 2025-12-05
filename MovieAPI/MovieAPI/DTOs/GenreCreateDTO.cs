using System.ComponentModel.DataAnnotations;
using MovieAPI.Validations;

namespace MovieAPI.DTOs;

public class GenreCreateDTO
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(maximumLength:50)]
    [FirstLetterAsCapital]
    public string Name { get; set; }
}