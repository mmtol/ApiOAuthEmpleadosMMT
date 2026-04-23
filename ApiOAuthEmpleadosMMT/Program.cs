using ApiOAuthEmpleadosMMT.Data;
using ApiOAuthEmpleadosMMT.Helpers;
using ApiOAuthEmpleadosMMT.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using MvcOAuthApiEmpleados.Helpers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();

HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);
//esta instancia solo debemos crearla 1 vez
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
//habilitamos la seguridad en el program
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJWTBearerOptions());

builder.Services.AddTransient<HelperCrytography>();
builder.Services.AddTransient<HelperEmpleadoToken>();

builder.Services.AddAzureClients(factory =>
{
    builder.Configuration.GetSection("KeyVault");
});

//este obj solo lo necesitamos aqui, recuperamos los valores y los asignamos a una clase
//recuperamos el secretclient
SecretClient client = builder.Services.BuildServiceProvider().GetService<SecretClient>();

//accedemos al secreto
KeyVaultSecret secreto = await client.GetSecretAsync("secretsqlazuremmt");

string connectionString = secreto.Value;
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryHospital>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
