using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Models;

namespace SupplierControlBackend.Contexts;

public partial class DbSC_SC_Suplier_In_Out : DbContext
{
    public DbSC_SC_Suplier_In_Out()
    {
    }

    public DbSC_SC_Suplier_In_Out(DbContextOptions<DbSC_SC_Suplier_In_Out> options)
        : base(options)
    {
    }

    public virtual DbSet<ScSuplierInOut> ScSuplierInOuts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.226.86;Database=dbSCM;TrustServerCertificate=True;uid=sa;password=decjapan");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<ScSuplierInOut>(entity =>
        {
            entity.HasKey(e => new { e.DeliveryDate, e.DeliveryShift, e.VenderCard, e.DeliveryRound });

            entity.ToTable("SC_Suplier_In_Out");

            entity.Property(e => e.DeliveryDate).HasColumnType("date");
            entity.Property(e => e.DeliveryShift).HasMaxLength(1);
            entity.Property(e => e.VenderCard).HasMaxLength(50);
            entity.Property(e => e.CreateBy).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EntryTime).HasColumnType("datetime");
            entity.Property(e => e.LeaveTime).HasColumnType("datetime");
            entity.Property(e => e.UpdateBy).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
