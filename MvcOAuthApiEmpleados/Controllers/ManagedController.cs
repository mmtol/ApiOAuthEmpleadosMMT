using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string token = await service.LogInAsync(model.Apellido, model.IdEmpleado);
            if (token == null)
            {
                ViewData["mensaje"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                ViewData["mensaje"] = "Bienvenido";
                ClaimsIdentity identity = new ClaimsIdentity
                    (
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name,
                        ClaimTypes.Role
                    );
                //almacenamos el nombre
                identity.AddClaim(new Claim(ClaimTypes.Name, model.Apellido));
                //almacenamos el id
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.IdEmpleado));
                //añadimos el token
                identity.AddClaim(new Claim("token", token));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                //damos de alta al usuario durante 20 min
                await HttpContext.SignInAsync
                    (
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                        }
                    );
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
