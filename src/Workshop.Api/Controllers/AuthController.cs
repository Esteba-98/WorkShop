using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Workshop.Application.DTOs.Auth;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nombre = dto.Nombre
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errores = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = string.Join(" ", errores) });
        }

        //Usar el rol enviado, o "User" si no se envía
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;

        // Si el rol no existe en BD, se crea automáticamente
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new AppRole { Name = role });
        }

        // Asignar rol al usuario
        await _userManager.AddToRoleAsync(user, role);

        return Ok(new
        {
            Message = "Usuario registrado correctamente",
            User = user.Email,
            Role = role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, dto.Password)))
            return Unauthorized("Credenciales inválidas");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        // Agregar todos los roles del usuario al token
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        return Ok(new AuthResponseDto
        {
            Id = user.Id.ToString(),
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList()
        });
    }
}
