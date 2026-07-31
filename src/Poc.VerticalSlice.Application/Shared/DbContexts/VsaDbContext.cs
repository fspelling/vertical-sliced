using Microsoft.EntityFrameworkCore;
using Poc.VerticalSlice.Application.Shared.Entities;

namespace Poc.VerticalSlice.Application.Shared.DbContexts;

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
