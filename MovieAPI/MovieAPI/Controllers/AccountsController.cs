using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.DTOs;
using MovieAPI.Utils;

namespace MovieAPI.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountsController: ControllerBase
{
    private UserManager<IdentityUser> _userManager;
    private IConfiguration _configuration;
    private SignInManager<IdentityUser> _signInManager;
    private ApplicationDBContext _context;
    private IMapper _mapper;

    public AccountsController(UserManager<IdentityUser>  userManager, SignInManager<IdentityUser> signInManager, 
        IConfiguration configuration, ApplicationDBContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _signInManager = signInManager;
        _configuration = configuration;
        _userManager = userManager;
    }

    [HttpGet("userlist")]
    public async Task<ActionResult<UserDTO>> GetUserList([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Users.AsQueryable();
        await HttpContext.InsertParameterIntoHeader(queryable);
        var user = await queryable.OrderBy(x => x.Email).Paginate(pagination).ToListAsync();
        return _mapper.Map<UserDTO>(user);
    }

    [HttpPost("makeadmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public async Task<ActionResult> MakeAdmin([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
        return NoContent();
    }
    
    [HttpPost("removeadmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
        return NoContent();
    }
    
    [HttpPost("create")]
    public async Task<ActionResult<RequestAuthentication>> Create([FromBody] UserCredencials userCredencials)
    {
        var user = new IdentityUser{UserName = userCredencials.Email, Email = userCredencials.Email};
        var result = await _userManager.CreateAsync(user, userCredencials.Password);

        if (result.Succeeded)
        {
            return await ConstructToken(userCredencials);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<RequestAuthentication>> Login([FromBody] UserCredencials userCredencials)
    {
        var result = await _signInManager.PasswordSignInAsync(userCredencials.Email, userCredencials.Password, false, false);

        if (result.Succeeded)
        {
            return await ConstructToken(userCredencials);
        }
        else
        {
            return BadRequest("Email or password is incorrect");
        }
    }
    private async Task<RequestAuthentication> ConstructToken(UserCredencials userCredencials)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", userCredencials.Email)
        };

        var user = await _userManager.FindByEmailAsync(userCredencials.Email);
        var claimsDB = await _userManager.GetClaimsAsync(user);
        
        claims.AddRange(claimsDB);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddYears(1);
        
        var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);
        
        return new RequestAuthentication(){Token = new JwtSecurityTokenHandler().WriteToken(token),  Expiration = expiration};
    }
}