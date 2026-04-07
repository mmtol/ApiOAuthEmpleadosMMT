using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiOAuthEmpleadosMMT.Helpers
{
    public class HelperActionOAuthService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperActionOAuthService(IConfiguration configuration)
        {
            Issuer = configuration.GetValue<string>("ApiOAuthToken:Issuer");
            Audience = configuration.GetValue<string>("ApiOAuthToken:Audience");
            SecretKey = configuration.GetValue<string>("ApiOAuthToken:SecretKey");
        }

        //necesitamos un metodo para generar el tojen a partir de nuestro secret key
        public SymmetricSecurityKey GetKeyToken()
        {
            //convertimos a bytes nuestro secret key
            byte[] data = Encoding.UTF8.GetBytes(SecretKey);
            return new SymmetricSecurityKey(data);
        }

        //utilizamos clases action para separar la capa de los services de autorizacion del program
        public Action<JwtBearerOptions> GetJWTBearerOptions()
        {
            //indicamos lo que va a validar dentro del token para permitir el acceso
            Action<JwtBearerOptions> options = new Action<JwtBearerOptions>(options =>
            {
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = GetKeyToken()
                };
            });

            return options;
        }

        //el esquema de nuestra validacion JwtBearerDefaults
        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options = new Action<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            return options;
        }
    }
}
