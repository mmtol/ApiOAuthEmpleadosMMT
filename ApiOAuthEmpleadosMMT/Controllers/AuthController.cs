using ApiOAuthEmpleadosMMT.Helpers;
using ApiOAuthEmpleadosMMT.Models;
using ApiOAuthEmpleadosMMT.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApiOAuthEmpleadosMMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperActionOAuthService helper;

        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper)
        {
            this.repo = repo;
            this.helper = helper;
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
                SigningCredentials credentials = new SigningCredentials(helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);
                //el token se genera con una clase y debemos almacenar los datos
                JwtSecurityToken token = new JwtSecurityToken
                    (
                        issuer: helper.Issuer,
                        audience: helper.Audience,
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
