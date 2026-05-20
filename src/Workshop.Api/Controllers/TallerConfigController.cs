using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.DTOs.TallerConfig;
using Workshop.Application.Services;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TallerConfigController : ControllerBase
    {
        private readonly ITallerConfigService _service;

        public TallerConfigController(ITallerConfigService service)
        {
            _service = service;
        }

        // GET /api/TallerConfig — obtener config actual (todos los roles, para el PDF)
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetAsync());
        }

        // PUT /api/TallerConfig — actualizar config (solo Administrador)
        [HttpPut]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update([FromBody] UpdateTallerConfigDto dto)
        {
            return Ok(await _service.UpdateAsync(dto));
        }
    }
}
