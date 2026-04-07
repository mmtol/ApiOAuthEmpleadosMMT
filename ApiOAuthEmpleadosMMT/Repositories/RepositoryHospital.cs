using ApiOAuthEmpleadosMMT.Data;
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
    }
}
