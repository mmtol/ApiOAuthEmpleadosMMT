using ApiOAuthEmpleadosMMT.Models;
using ApiOAuthEmpleadosMMT.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Helpers;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleadosMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperCrytography helperCrypt;
        private IConfiguration conf;

        public EmpleadosController(RepositoryHospital repo, HelperCrytography helperCrypt, IConfiguration conf)
        {
            this.repo = repo;
            this.helperCrypt = helperCrypt;
            this.conf = conf;
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
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonEmp = helperCrypt.Decrypt(claim.Value, conf.GetValue<string>("KeyCryt"));
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmp);
            return await repo.FindEmpleadoAsync(empleado.IdEmpleado);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonEmp = claim.Value;
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmp);
            return await repo.GetCompisAsync(empleado.IdDepartamento);
        }
    }
}
