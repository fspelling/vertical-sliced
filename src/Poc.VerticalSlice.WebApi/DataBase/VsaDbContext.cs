using Microsoft.EntityFrameworkCore;
using Poc.VerticalSlice.WebApi.Entities;

namespace Poc.VerticalSlice.WebApi.DataBase
{
    public class VsaDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }

        public VsaDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasKey(t => t.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
