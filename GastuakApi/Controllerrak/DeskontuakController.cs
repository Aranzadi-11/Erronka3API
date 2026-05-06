using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeskontuakController : ControllerBase
    {
        private readonly DeskuntuakRepository _repo;

        public DeskontuakController(DeskuntuakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Deskontu kode bat baliozkoa den egiaztatzen du Odoo datu-baseko arauak erabilita.
        /// </summary>
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] DeskuntuKodeaDto? dto)
        {
            return Ok(await _repo.ValidateAsync(dto?.GetCode()));
        }

        /// <summary>
        /// Deskontu kode bat balidatu eta, zuzena bada, erabilera kopurua handitzen du.
        /// </summary>
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] DeskuntuKodeaDto? dto)
        {
            return Ok(await _repo.ApplyAsync(dto?.GetCode()));
        }
    }
}
