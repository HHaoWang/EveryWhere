using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using static System.Net.Mime.MediaTypeNames;

#nullable disable

namespace EveryWhere.Database;

public class Repository : DbContext
{
    public virtual DbSet<PO.File> File { get; set; }
    public virtual DbSet<PO.Order> Orders { get; set; }
    public virtual DbSet<PO.Consumer> Consumers { get; set; }
    public virtual DbSet<PO.PrintJob> PrintJobs { get; set; }
    public virtual DbSet<PO.Printer> Printers { get; set; }
    public virtual DbSet<PO.Shop> Shops { get; set; }

    public Repository(){}

    public Repository(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //仅用于开发时快速修改数据库而配置，二次开发需要修改下方参数
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

        modelBuilder.Entity<PO.Consumer>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<PO.Printer>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<PO.PrintJob>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<PO.Shop>(entity =>
        {
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}