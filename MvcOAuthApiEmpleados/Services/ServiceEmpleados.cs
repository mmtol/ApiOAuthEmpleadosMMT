using MvcOAuthApiEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MvcOAuthApiEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string url;
        private MediaTypeWithQualityHeaderValue header;

        public ServiceEmpleados(IConfiguration configuration)
        {
            url = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            //url = "https://apioauthempleadosmmt.azurewebsites.net/";
            header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        private async Task<string> LogInAsync(string apellido, string idEmpleado)
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

        //por ahora se recibe el token en el metodo
        public async Task<Empleado> FindEmpleadoAsync(int idEmpleado, string token)
        {
            string request = $"api/empleados/{idEmpleado}";
            Empleado empleado = await CallApiAsync<Empleado>(request, token);
            return empleado;
        }
    }
}
