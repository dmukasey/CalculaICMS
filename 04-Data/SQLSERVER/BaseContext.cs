using Microsoft.EntityFrameworkCore;

namespace SQLSERVER
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options)
           : base(options)
        { }

       // public DbSet<davidsoftware.Models.Licenciamento> licenciamento { get; set; }

       

    }
}
