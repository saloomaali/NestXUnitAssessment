using Microsoft.EntityFrameworkCore;
using XUnitAssessment.API.Models;

namespace XUnitAssessment.API.Data
{


    public partial class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Field> Field { get; set; }

        public DbSet<Form> Form { get; set; }

        public DbSet<AOColumn> AOColumn { get; set; }

        public DbSet<AOTable> AOTable { get; set; }
    }
}