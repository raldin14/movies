using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using MovieAPI.Utils;

namespace MovieAPI.Controllers;
[ApiController]
[Route("api/movies")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
public class MoviesCotroller: ControllerBase
{
    private ApplicationDBContext _context;
    private IMapper _mapper;
    private IStorageAzureStorage _storage;
    private readonly string _container = "movies";

    public MoviesCotroller(ApplicationDBContext context, IMapper mapper, IStorageAzureStorage  storage)
    {
        _storage = storage;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<LandingPageDTO>> Get()
    {
        var top = 6;
        var today =  DateTime.Today;
        
        var comingUp = await _context.Movies
            .Where(x => x.releaseDate > today)
            .OrderBy(x => x.releaseDate)
            .Take(top)
            .ToListAsync();
        
        var nowPlaying = await _context.Movies
            .Where(x => x.nowPlaying)
            .OrderBy(x => x.releaseDate)
            .Take(top)
            .ToListAsync();

        var result = new LandingPageDTO();
        
        result.ComingUp = _mapper.Map<List<MovieDTO>>(comingUp);
        result.NowPlaying = _mapper.Map<List<MovieDTO>>(nowPlaying);
        return result;
    }
    
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<MovieDTO>> Get(int id)
    {
        var movie = await _context.Movies
            .Include(x => x.MoviesGenres).ThenInclude(x => x.genre)
            .Include(x => x.MoviesActors).ThenInclude(x => x.actor)
            .Include(x => x.MoviesTheathers).ThenInclude(x => x.theater)
            .FirstOrDefaultAsync(x => x.id == id);

        if (movie == null) return NotFound();
        
        var dto = _mapper.Map<MovieDTO>(movie);
        dto.actors = dto.actors.OrderBy(x => x.order).ToList();
        return dto;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromForm] MovieCreateDTO movieCreateDto)
    {
        var movie = _mapper.Map<Movie>(movieCreateDto);
        
        if (movieCreateDto.poster != null)
        {
            movie.poster =  await _storage.SaveFile(_container, movieCreateDto.poster);
        }
        
        WriteOrderActors(movie);
        _context.Add(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("postget")]
    public async Task<ActionResult<MoviesPostGetDTO>> PostGet()
    {
        var theather = await _context.Theathers.ToListAsync();
        var genres = await _context.Genres.ToListAsync();
        
        var theatherDTO = _mapper.Map<List<TheatherDTO>>(theather);
        var genresDTO = _mapper.Map<List<GenresDTO>>(genres);
        
        return new MoviesPostGetDTO(){Theather = theatherDTO, Genres = genresDTO};
    }
    
    [HttpGet("PutGet/{id:int}")]
    public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
    {
        var movieActionResult = await Get(id);
        if (movieActionResult.Result is NotFoundResult)
        {
            return NotFound();
        }

        var movie = movieActionResult.Value;
        
        var selectedGenres = movie.genres.Select(x => x.Id).ToList();
        var noSelectedGenres = await _context.Genres
            .Where(x => !selectedGenres.Contains(x.id))
            .ToListAsync();
        
        var selectedTheathers = movie.theather.Select(x => x.id).ToList();
        var noSelectedTheathers = await _context.Theathers
            .Where(x => !selectedTheathers.Contains(x.id))
            .ToListAsync();
        
        var noSelectedGenresDTO = _mapper.Map<List<GenresDTO>>(noSelectedGenres);
        var noSelectedTheatersDTO = _mapper.Map<List<TheatherDTO>>(noSelectedTheathers);

        var response = new MoviePutGetDTO();
        response.Movie = movie;
        response.SelectedGenres = movie.genres;
        response.NoSelectedGenres = noSelectedGenresDTO;
        response.SelectedTheathers = movie.theather;
        response.NoSelectedTheathers = noSelectedTheatersDTO;
        response.Actors = movie.actors;

        return response;
    }

    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MovieFilterDTO filterDTO)
    {
        var movieQueryable =  _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(filterDTO.title))
        {
            movieQueryable = movieQueryable.Where(x => x.title.ToLower().Contains(filterDTO.title.ToLower()));
        }

        if (filterDTO.nowPlaying)
        {
            movieQueryable = movieQueryable.Where(x => x.nowPlaying);
        }

        if (filterDTO.comingUp)
        {
            var today =  DateTime.Today;
            movieQueryable = movieQueryable.Where(x => x.releaseDate > today);
        }

        if (filterDTO.genreId != 0)
        {
            movieQueryable = movieQueryable.Where(x => x.MoviesGenres.Select(x =>  x.genreId).Contains(filterDTO.genreId));
        }
        
        await HttpContext.InsertParameterIntoHeader(movieQueryable);
        var movie = await movieQueryable.Paginate(filterDTO.pagination).ToListAsync();
        return _mapper.Map<List<MovieDTO>>(movie);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm]  MovieCreateDTO movieCreateDto)
    {
        var movie = await _context.Movies
            .Include(x => x.MoviesGenres)
            .Include(x => x.MoviesActors)
            .Include(x => x.MoviesTheathers)
            .FirstOrDefaultAsync(x => x.id == id);
        
        if(movie == null) return NotFound();
        
        movie = _mapper.Map(movieCreateDto, movie);

        if (movieCreateDto.poster != null)
        {
            movie.poster =  await _storage.EditFile(_container, movie.poster, movieCreateDto.poster);
        }
        
        WriteOrderActors(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    private void WriteOrderActors(Movie movie)
    {
        if (movie.MoviesActors != null)
        {
            for (int i = 0; i < movie.MoviesActors.Count; i++)
            {
                movie.MoviesActors[i].order = i;
            }
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(x => x.id == id);

        if (movie == null) return NotFound();

        _context.Remove(movie);
        await _context.SaveChangesAsync();
        await _storage.DeleteFile(movie.poster, _container);
        return NoContent();
    }
}