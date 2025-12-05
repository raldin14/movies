using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using MovieAPI.Migrations;

namespace MovieAPI.Controllers;
[Route("api/ratings")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RatingsConttroller: ControllerBase
{
    private UserManager<IdentityUser> _userManager;
    private ApplicationDBContext _context;

    public RatingsConttroller(UserManager<IdentityUser> userManager, ApplicationDBContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post([FromBody] RatingDTO ratingsDTO)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        var user = await _userManager.FindByEmailAsync(email);
        var userId = user.Id;

        var actualRating =
            await _context.Ratings.FirstOrDefaultAsync(x => x.movieId == ratingsDTO.MovieId && x.userId == userId);

        if (actualRating == null)
        {
            var rating = new Rating();
            rating.movieId = ratingsDTO.MovieId;
            rating.rate = ratingsDTO.Rate;
            rating.userId = userId;
            _context.Add(rating);
        }
        else
        {
            actualRating.rate = ratingsDTO.Rate;
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }
}