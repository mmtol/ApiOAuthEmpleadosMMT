using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await service.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details(int id)
        {
            //tendremos el token en session
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                ViewData["mensaje"] = "Hay que hacer Log In";
                return View();
            }
            else
            {
                Empleado empleado = await service.FindEmpleadoAsync(id, token);
                return View(empleado);
            }
        }
    }
}