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

        public EmpleadosController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleados()
        {
            return await repo.GetEmpleadosAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Empleado>> FindEmpleado(int id)
        {
            Empleado empleado = await repo.FindEmpleadoAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return empleado;
        }
    }
}
