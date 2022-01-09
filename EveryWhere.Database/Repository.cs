using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using static System.Net.Mime.MediaTypeNames;

#nullable disable

namespace EveryWhere.Database;

public class Repository : DbContext
{
    public virtual DbSet<PO.File> File { get; set; }
    public virtual DbSet<PO.Order> Order { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql("server = 127.0.0.1; uid = EveryWhere; pwd = EveryWhere; database = every_where", 
                ServerVersion.Parse("5.7.26-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PO.File>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<PO.Order>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}