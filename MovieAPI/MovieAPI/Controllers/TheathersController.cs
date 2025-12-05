using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Entity;
using MovieAPI.Utils;

namespace MovieAPI.Controllers;
[ApiController]
[Route("api/theathers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
public class TheathersController: ControllerBase
{
    private ApplicationDBContext _context;
    private IMapper _mapper;

    public TheathersController(ApplicationDBContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TheatherDTO>>> GetAll([FromQuery] PaginationDTO pagination)
    {
        var queriable = _context.Theathers.AsQueryable();
        await HttpContext.InsertParameterIntoHeader(queriable);
        var theather = await queriable.OrderBy(x => x.name).Paginate(pagination).ToListAsync();
        return _mapper.Map<List<TheatherDTO>>(theather);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TheatherDTO>> Get(int id)
    {
        var theather = await _context.Theathers.FirstOrDefaultAsync(x => x.id == id);

        if (theather == null)
        {
            return NotFound();
        }
        
        return _mapper.Map<TheatherDTO>(theather);
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] TheatherCreateDTO theatherCreateDto)
    {
        var theather = _mapper.Map<Theather>(theatherCreateDto);
        _context.Add(theather);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] TheatherCreateDTO theatherCreateDto)
    {
        var theather = await _context.Theathers.FirstOrDefaultAsync(x => x.id == id);

        if (theather == null) return NotFound();
        
        theather = _mapper.Map(theatherCreateDto, theather);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var exist = await _context.Theathers.AnyAsync(x => x.id == id);

        if (!exist)
        {
            return NotFound();
        }

        _context.Remove(new Theather() { id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}