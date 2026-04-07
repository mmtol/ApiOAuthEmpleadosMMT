using Microsoft.EntityFrameworkCore;

namespace ApiOAuthEmpleadosMMT.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options)
        {
        }
        public DbSet<Models.Empleado> Empleados { get; set; }
    }
}
