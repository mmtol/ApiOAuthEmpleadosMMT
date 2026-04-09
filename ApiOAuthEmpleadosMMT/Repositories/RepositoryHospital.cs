using ApiOAuthEmpleadosMMT.Data;
using ApiOAuthEmpleadosMMT.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOAuthEmpleadosMMT.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Models.Empleado>> GetEmpleadosAsync()
        {
            return await context.Empleados.ToListAsync();
        }

        public async Task<Models.Empleado> FindEmpleadoAsync(int id)
        {
            return await context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == id);
        }

        public async Task<Empleado> LogInEmpleadoAsync(string apellido, int id)
        {
            return await context.Empleados.Where(x => x.Apellido == apellido && x.IdEmpleado == id).FirstOrDefaultAsync();
        }

        public async Task<List<Empleado>> GetCompisAsync(int id)
        {
            return await context.Empleados.Where(x => x.IdDepartamento == id).ToListAsync();
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in context.Empleados
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosByOficios(List<string> oficios)
        {
            var consulta = from datos in context.Empleados
                           where oficios.Contains(datos.Oficio)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task IncrementarSalarioAsync(int incremento, List<string> oficios)
        {
            List<Empleado> empleados = await GetEmpleadosByOficios(oficios);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }

            await context.SaveChangesAsync();
        }
    }
}
