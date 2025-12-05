using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using MovieAPI.Utils;

namespace MovieAPI.Controllers;
[Route("api/actors")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
public class ActorsController:ControllerBase
{
    private ApplicationDBContext _context;
    private IMapper _mapper;
    private IStorageAzureStorage _storage;
    private readonly string _container = "actors";
    public ActorsController(ApplicationDBContext context, IMapper mapper, IStorageAzureStorage  storage)
    {
        _storage = storage;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO pagination)
    {
        var queriable =  _context.Actors.AsQueryable();
        await HttpContext.InsertParameterIntoHeader(queriable);
        var actors = await queriable.OrderBy(x => x.name).Paginate(pagination).ToListAsync();
        return _mapper.Map < List<ActorDTO>>(actors);
    }

    [HttpGet("searchByName/{name}")]
    public async Task<ActionResult<List<MovieActorDTO>>> SearchByName(string name = "")
    {
        if(string.IsNullOrWhiteSpace(name)) {return new List<MovieActorDTO>();}

        return await _context.Actors.Where(x => x.name.ToLower().Contains(name.ToLower()))
            .OrderBy(x => x.name)
            .Select(x => new MovieActorDTO() { id = x.id, name = x.name, picture = x.picture })
            .Take(5)
            .ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ActorDTO>> Get(int id)
    {
        var actor = await _context.Actors.FirstOrDefaultAsync(x => x.id == id);
        if (actor == null)
        {
            return NotFound();
        }
        return _mapper.Map<ActorDTO>(actor);
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromForm] ActorCreateDTO actorDTO)
    {
        var actor = _mapper.Map<Actor>(actorDTO);

        if (actorDTO.Picture != null)
        {
           actor.picture =  await _storage.SaveFile(_container, actorDTO.Picture);
        }
        _context.Add(actor);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromForm] ActorCreateDTO actorDTO)
    {
        var actor = _context.Actors.FirstOrDefault(x => x.id == id);
        if (actor == null)
        {
            return NotFound();
        }
        actor = _mapper.Map(actorDTO, actor);

        if (actorDTO.Picture.Length > 0)
        {
            actor.picture =  await _storage.EditFile(_container,actor.picture, actorDTO.Picture);
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var actor = await _context.Actors.FirstOrDefaultAsync(x => x.id == id);

        if (actor == null)
        {
            return NotFound();
        }
        
        _context.Remove(actor);
        await _context.SaveChangesAsync();
        await _storage.DeleteFile(_container, actor.picture);
        return NoContent();
    }
    
    
}