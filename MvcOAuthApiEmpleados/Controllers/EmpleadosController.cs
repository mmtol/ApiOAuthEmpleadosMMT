using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Filters;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int id)
        {
            Empleado empleado = await service.FindEmpleadoAsync(id);
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Perfil()
        {
            Empleado empleado = await service.GetPerfilAsync();
            return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            List<Empleado> compis = await service.GetCompisAsync();
            return View(compis);
        }

        public async Task<IActionResult> EmpleadosOficios()
        {
            List<string> oficios = await service.GetOficiosAsync();
            ViewData["oficios"] = oficios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficios(int? incremento, string accion, List<string> elegidos)
        {
            List<string> oficios = await service.GetOficiosAsync();
            ViewData["oficios"] = oficios;

            if (accion == "update")
            {
                await service.UpdateEmpleadosAsync(incremento.Value, elegidos);
            }

            List<Empleado> empleados = await service.GetEmpleadosOficioAsync(elegidos);
            return View(empleados);
        }
    }
}