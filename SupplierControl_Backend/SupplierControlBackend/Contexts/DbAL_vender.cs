using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Models;

namespace SupplierControlBackend.Contexts;

public partial class DbAL_vender : DbContext
{
    public DbAL_vender()
    {
    }

    public DbAL_vender(DbContextOptions<DbAL_vender> options)
        : base(options)
    {
    }

    public virtual DbSet<AlVendor> AlVendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.226.86;Database=dbSCM;TrustServerCertificate=True;uid=sa;password=decjapan");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<AlVendor>(entity =>
        {
            entity.HasKey(e => e.Vender);

            entity.ToTable("AL_Vendor");

            entity.Property(e => e.Vender).HasMaxLength(20);
            entity.Property(e => e.AbbreName).HasMaxLength(50);
            entity.Property(e => e.Boitype)
                .HasMaxLength(10)
                .HasColumnName("BOIType");
            entity.Property(e => e.Currency).HasMaxLength(5);
            entity.Property(e => e.EmailPo)
                .HasMaxLength(300)
                .HasColumnName("EMailPO");
            entity.Property(e => e.IsMilkRun)
                .HasDefaultValueSql("((0))")
                .HasColumnName("isMilkRun");
            entity.Property(e => e.PersonIncharge).HasMaxLength(50);
            entity.Property(e => e.Route).HasMaxLength(1);
            entity.Property(e => e.VenderCard).HasMaxLength(20);
            entity.Property(e => e.VenderName).HasMaxLength(300);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
