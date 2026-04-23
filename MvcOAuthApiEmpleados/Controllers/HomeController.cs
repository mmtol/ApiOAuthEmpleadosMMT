using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Models;
using System.Diagnostics;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class HomeController : Controller
    {
        private SecretClient client;

        public HomeController(SecretClient client)
        {
            this.client = client;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name)
        {
            KeyVaultSecret secreto = await client.GetSecretAsync(name);
            ViewData["secreto"] = secreto.Value;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
