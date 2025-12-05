using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace MovieAPI.Entity;

public class Theather
{
    public int id { get; set; }
    [Required]
    [StringLength(maximumLength:75)]
    public string name { get; set; }
    public Point location { get; set; }
    public List<MoviesTheathers> MoviesTheathers { get; set; }
}