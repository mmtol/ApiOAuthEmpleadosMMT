
//para crear metodos en program debemos hacerlos static
using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

Console.WriteLine("Cliente Api Auth");
Console.WriteLine("Introduzca apellido");
string apellido = Console.ReadLine();
Console.WriteLine("Introduzca Id");
string idEmpleado = Console.ReadLine();
string respuesta = await GetTokenAsync(apellido, idEmpleado);
Console.WriteLine(respuesta);
Console.WriteLine("-------------------------------------------");
Console.WriteLine("Introduzca ID a buscar");
string idBuscar = Console.ReadLine();
string response = await FindEmpleadoAsync(int.Parse(idBuscar), respuesta);
Console.WriteLine(response);
Console.WriteLine("-------------------------------------------");


static async Task<string> GetTokenAsync(string apellido, string idEmpleado)
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

static async Task<string> FindEmpleadoAsync(int idEmpleado, string token)
{
    using (HttpClient client = new HttpClient())
    {
        string url = "https://apioauthempleadosmmt.azurewebsites.net/";
        string request = $"api/empleados/{idEmpleado}";
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await client.GetAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }
        else
        {
            return "Peticion incorrecta:" + response.StatusCode;
        }
    }
}