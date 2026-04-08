using MvcOAuthApiEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string url;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor context;

        public ServiceEmpleados(IConfiguration configuration, IHttpContextAccessor context)
        {
            url = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            header = new MediaTypeWithQualityHeaderValue("application/json");
            this.context = context;
        }

        public async Task<string> LogInAsync(string apellido, string idEmpleado)
        {
            string url = "https://apioauthempleadosmmt.azurewebsites.net/";
            LoginModel loginModel = new LoginModel
            {
                Apellido = apellido,
                IdEmpleado = idEmpleado
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string json = JsonConvert.SerializeObject(loginModel);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(data);
                    string token = obj.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return "Peticion incorrecta:" + response.StatusCode;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        //realizamos una sobrecarga
        public async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/empleados";
            List<Empleado> empleados = await CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
        {
            string request = $"api/empleados/{idEmpleado}";
            string token = context.HttpContext.User.FindFirstValue("token");

            Empleado empleado = await CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<Empleado> GetPerfilAsync()
        {
            string token = context.HttpContext.User.FindFirstValue("token");
            string request = "api/empleados/perfil";

            Empleado empleado = await CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<List<Empleado>> GetCompisAsync()
        {
            string token = context.HttpContext.User.FindFirstValue("token");
            string request = "api/empleados/compis";

            List<Empleado> compis = await CallApiAsync<List<Empleado>>(request, token);
            return compis;
        }
    }
}
