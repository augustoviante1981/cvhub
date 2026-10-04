using cvhub.Models;
using Microsoft.EntityFrameworkCore;

namespace cvhub.Data
{
    public class cvhubDbContext : DbContext
    {
        public cvhubDbContext(DbContextOptions<cvhubDbContext> options)
        : base(options)
        {
        }

        public DbSet<Candidato> Candidatos { get; set; }
    }
}
