using ApiOAuthEmpleadosMMT.Helpers;
using ApiOAuthEmpleadosMMT.Models;
using ApiOAuthEmpleadosMMT.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiOAuthEmpleadosMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperEmpleadoToken helperEmpleadoToken;

        public EmpleadosController(RepositoryHospital repo, HelperEmpleadoToken helperEmpleadoToken)
        {
            this.repo = repo;
            this.helperEmpleadoToken = helperEmpleadoToken;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await repo.GetEmpleadosAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            Empleado empleado = await repo.FindEmpleadoAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return empleado;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> Perfil()
        {
            EmpleadoModel empleado = helperEmpleadoToken.GetEmpleado();
            return await repo.FindEmpleadoAsync(empleado.IdEmpleado);
        }

        [Authorize(Roles = "PRESIDENTE")]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            EmpleadoModel empleado = helperEmpleadoToken.GetEmpleado();
            return await repo.GetCompisAsync(empleado.IdDepartamento);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            return await repo.GetOficiosAsync();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> EmpleadosByOficios([FromQuery] List<string> oficios)
        {
            return await repo.GetEmpleadosByOficios(oficios);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> IncrementarSalario(int incremento, List<string> oficios)
        {
            await repo.IncrementarSalarioAsync(incremento, oficios);
            return Ok();
        }
    }
}
