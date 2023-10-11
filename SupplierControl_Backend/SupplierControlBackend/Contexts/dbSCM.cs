using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SupplierControlBackend.Models;

namespace SupplierControlBackend.Contexts;

public partial class dbSCM : DbContext
{
    public dbSCM()
    {
    }

    public dbSCM(DbContextOptions<dbSCM> options)
        : base(options)
    {
    }

    public virtual DbSet<ScSupplierDriverLisense> ScSupplierDriverLisenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.226.86;Database=dbSCM;TrustServerCertificate=True;uid=sa;password=decjapan");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<ScSupplierDriverLisense>(entity =>
        {
            entity.HasKey(e => e.DriverNbr).HasName("PK_SC_Suplier_Driver_Lisense");

            entity.ToTable("SC_Supplier_Driver_Lisense");

            entity.Property(e => e.DriverNbr).HasMaxLength(50);
            entity.Property(e => e.CreateBy).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DriverLicence).HasMaxLength(50);
            entity.Property(e => e.DriverLicenceExpire).HasColumnType("datetime");
            entity.Property(e => e.DriverPicture).HasMaxLength(300);
            entity.Property(e => e.DriverStatus).HasMaxLength(50);
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .HasColumnName("FName");
            entity.Property(e => e.IssueBy).HasMaxLength(50);
            entity.Property(e => e.IssueDate).HasColumnType("date");
            entity.Property(e => e.Surn).HasMaxLength(50);
            entity.Property(e => e.TcTel).HasMaxLength(10);
            entity.Property(e => e.UpdateBy).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.VehicleNbr).HasMaxLength(50);
            entity.Property(e => e.VenderCode).HasMaxLength(50);
            entity.Property(e => e.VenderName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
