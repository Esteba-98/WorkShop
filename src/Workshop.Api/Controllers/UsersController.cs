using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Api.Controllers
{
    public class ChangePasswordDto { public string NewPassword { get; set; } = ""; }
    public class ChangeRoleDto { public string Role { get; set; } = ""; }
    public class UpdateProfileDto { public string Nombre { get; set; } = ""; public string Email { get; set; } = ""; }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET /api/Users — lista todos los usuarios con su rol (solo Administrador)
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAll()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new
                {
                    id = u.Id,
                    nombre = u.Nombre,
                    email = u.Email,
                    rol = roles.FirstOrDefault() ?? "User"
                });
            }
            return Ok(result);
        }

        // GET /api/Users/mecanicos — para el selector de mecánicos en mantenimientos
        [HttpGet("mecanicos")]
        [Authorize(Roles = "Administrador,User")]
        public async Task<IActionResult> GetMecanicos()
        {
            var mecanicos = await _userManager.GetUsersInRoleAsync("Mecanico");
            var result = mecanicos.Select(u => new
            {
                id = u.Id,
                nombre = u.Nombre,
                email = u.Email
            });
            return Ok(result);
        }

        // PUT /api/Users/{id}/profile — actualizar nombre y email (solo Administrador)
        [HttpPut("{id}/profile")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            user.Nombre = dto.Nombre;
            user.UserName = dto.Email;
            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpperInvariant();
            user.NormalizedUserName = dto.Email.ToUpperInvariant();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errores = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errores) });
            }
            return Ok(new { message = "Perfil actualizado." });
        }

        // PUT /api/Users/{id}/role — cambiar rol (solo Administrador)
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var rolesActuales = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesActuales);

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new AppRole { Name = dto.Role });

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
            {
                var errores = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errores) });
            }
            return Ok(new { message = "Rol actualizado." });
        }

        // PUT /api/Users/{id}/change-password — cambiar contraseña (solo Administrador)
        [HttpPut("{id}/change-password")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errores = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errores) });
            }
            return Ok(new { message = "Contraseña actualizada." });
        }

        // DELETE /api/Users/{id} — eliminar usuario del sistema (solo Administrador)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errores = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = string.Join(" ", errores) });
            }
            return NoContent();
        }
    }
}
