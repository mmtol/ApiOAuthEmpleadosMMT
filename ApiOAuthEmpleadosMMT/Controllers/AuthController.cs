using ApiOAuthEmpleadosMMT.Helpers;
using ApiOAuthEmpleadosMMT.Models;
using ApiOAuthEmpleadosMMT.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MvcOAuthApiEmpleados.Helpers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiOAuthEmpleadosMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperActionOAuthService helperService;
        private HelperCrytography helperCrypt;
        private IConfiguration conf;

        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper, HelperCrytography helperCrypt, IConfiguration conf)
        {
            this.repo = repo;
            this.helperService = helper;
            this.helperCrypt = helperCrypt;
            this.conf = conf;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> LogIn(LoginModel model)
        {
            Empleado empleado = await repo.LogInEmpleadoAsync(model.Apellido, int.Parse(model.IdEmpleado));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //debemos crear unas credenciales con nuestro token
                SigningCredentials credentials = new SigningCredentials(helperService.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                //creamos nuestro modelo para almacenarlo en el token
                EmpleadoModel empModel = new EmpleadoModel
                {
                    IdEmpleado = empleado.IdEmpleado,
                    Apellido = empleado.Apellido,
                    Oficio = empleado.Oficio,
                    Salario = empleado.Salario,
                    IdDepartamento = empleado.IdDepartamento
                };

                //almacenamos el empleado en los claims
                string jsonEmp = JsonConvert.SerializeObject(empModel);
                //encriptamos el json del empleado
                jsonEmp = helperCrypt.Encrypt(jsonEmp, conf.GetValue<string>("KeyCryt"));
                //creamos un array de claims para el token
                Claim[] inf = new[]
                {
                    new Claim("UserData", jsonEmp),
                    new Claim(ClaimTypes.Role, empleado.Oficio)
                };

                //el token se genera con una clase y debemos almacenar los datos
                JwtSecurityToken token = new JwtSecurityToken
                    (
                        claims: inf,
                        issuer: helperService.Issuer,
                        audience: helperService.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow
                    );
                //por ultimo, dveolvemos la respuesta afirmativa con el token
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
