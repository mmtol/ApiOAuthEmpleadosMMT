using ApiOAuthEmpleadosMMT.Models;
using MvcOAuthApiEmpleados.Helpers;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleadosMMT.Helpers
{
    public class HelperEmpleadoToken
    {
        private IHttpContextAccessor context;
        private HelperCrytography helper;
        private IConfiguration conf;

        public HelperEmpleadoToken(IHttpContextAccessor context, HelperCrytography helper, IConfiguration conf)
        {
            this.context = context;
            this.helper = helper;
            this.conf = conf;
        }

        public EmpleadoModel GetEmpleado()
        {
            Claim claim = context.HttpContext.User.FindFirst(z => z.Type == "UserData");
            string json = claim.Value;
            string jsonEmpleado = helper.Decrypt(json, conf.GetValue<string>("KeyCryt"));
            EmpleadoModel model = JsonConvert.DeserializeObject<EmpleadoModel>(jsonEmpleado);
            return model;
        }
    }
}
