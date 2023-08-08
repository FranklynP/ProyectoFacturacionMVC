using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFacturacionMVC.Models;

public partial class ProyectoFacturacionDbContext : DbContext
{
    public ProyectoFacturacionDbContext()
    {
    }

    public ProyectoFacturacionDbContext(DbContextOptions<ProyectoFacturacionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<ArticulosFactura> ArticulosFacturas { get; set; }

    public virtual DbSet<AsientoContable> AsientoContables { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<CuentaContable> CuentaContables { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("server=LAPTOP-9B1RG76E\\SQLEXPRESS; database=ProyectoFacturacionDB; integrated security=true; TrustServerCertificate=true;");
    {
        if (!optionsBuilder.IsConfigured)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //            optionsBuilder.UseSqlServer("server=LAPTOP-9B1RG76E\\SQLEXPRESS; database=ProyectoFacturacionDB; integrated security=true; TrustServerCertificate=true;");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Articulo__3213E83FCFDC37FA");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<ArticulosFactura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Articulo__3213E83F9CBCEAF6");

            entity.ToTable("ArticulosFactura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.IdArticulo).HasColumnName("id_articulo");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("precio_unitario");
        });

        modelBuilder.Entity<AsientoContable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AsientoC__3213E83F6EABE9BF");

            entity.ToTable("AsientoContable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CuentaCr).HasColumnName("cuenta_cr");
            entity.Property(e => e.CuentaDb).HasColumnName("cuenta_db");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("monto");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3213E83FAA0457EF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Cedula)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CuentaContableId).HasColumnName("cuenta_contable_id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Rnc).HasColumnName("rnc");
        });

        modelBuilder.Entity<CuentaContable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CuentaCo__3213E83FF95DB7D7");

            entity.ToTable("CuentaContable");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empleado__3213E83FD0D0768A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("((1))")
                .HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Comision).HasColumnName("comision");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Facturas__3213E83FEBD9469E");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comentario)
                .HasColumnType("text")
                .HasColumnName("comentario");
            entity.Property(e => e.Comprobante).HasColumnName("comprobante");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdVendedor).HasColumnName("id_vendedor");
            entity.Property(e => e.Itbis)
                .HasDefaultValueSql("((0.18))")
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("itbis");
            entity.Property(e => e.NumFactura).HasColumnName("num_factura");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.TipoFactura)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('venta')")
                .HasColumnName("tipo_factura");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("total");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
