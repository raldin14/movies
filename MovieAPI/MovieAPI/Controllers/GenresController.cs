using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using MovieAPI.Repository;
using MovieAPI.Utils;

namespace MovieAPI.Controllers;
[Route("api/genres")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
public class GenresController: ControllerBase
{
    private ILogger<GenresController> _logger;
    private ApplicationDBContext _context;
    private IMapper _mapper;


    public GenresController(ILogger<GenresController> logger, ApplicationDBContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GenresDTO>>> Get([FromQuery] PaginationDTO pagination)
    {
        var queriable =  _context.Genres.AsQueryable();
        await HttpContext.InsertParameterIntoHeader(queriable);
        var genres = await queriable.OrderBy(x => x.name).Paginate(pagination).ToListAsync();
        return _mapper.Map<List<GenresDTO>>(genres);
    }

    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GenresDTO>>> GetAll()
    {
        var genres = await _context.Genres.OrderBy(x => x.name).ToListAsync();
        return _mapper.Map<List<GenresDTO>>(genres);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenresDTO>> Get(int id)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(x => x.id == id);

        if (genre == null)
        {
            return NotFound();
        }
        
        return _mapper.Map<GenresDTO>(genre);
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] GenreCreateDTO genrecreateDTO)
    {
        var genre = _mapper.Map<Genre>(genrecreateDTO);
        _context.Add(genre);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] GenreCreateDTO genreCreateDto)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(x => x.id == id);

        if (genre == null) return NotFound();
        
        genre = _mapper.Map(genreCreateDto, genre);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var exist = await _context.Genres.AnyAsync(x => x.id == id);
        if(!exist) return NotFound();
        
        _context.Remove(new Genre() { id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}