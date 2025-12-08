using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiMonitoreo.Models;

public partial class MonitoreoContext : DbContext
{
    public MonitoreoContext()
    {
    }

    public MonitoreoContext(DbContextOptions<MonitoreoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<ConsumoMateriaPrima> ConsumoMateriaPrimas { get; set; }

    public virtual DbSet<HistorialPrueba> HistorialPruebas { get; set; }

    public virtual DbSet<LoteMateriaPrima> LoteMateriaPrimas { get; set; }

    public virtual DbSet<MateriaPrima> MateriaPrimas { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<Produccion> Produccions { get; set; }

    public virtual DbSet<ProductoTerminado> ProductoTerminados { get; set; }

    public virtual DbSet<Prueba> Pruebas { get; set; }

    public virtual DbSet<PruebaProducto> PruebaProductos { get; set; }

    public virtual DbSet<RecetaProducto> RecetaProductos { get; set; }

    public virtual DbSet<SerieProducto> SerieProductos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CP");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConsumoMateriaPrima>(entity =>
        {
            entity.HasKey(e => e.ConsumoId).HasName("PK__ConsumoM__206D9CC68F78F572");

            entity.ToTable("ConsumoMateriaPrima");

            entity.Property(e => e.ConsumoId).HasColumnName("ConsumoID");
            entity.Property(e => e.CantidadUsada).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoteId).HasColumnName("LoteID");
            entity.Property(e => e.MateriaPrimaId).HasColumnName("MateriaPrimaID");
            entity.Property(e => e.ProduccionId).HasColumnName("ProduccionID");

            entity.HasOne(d => d.Lote).WithMany(p => p.ConsumoMateriaPrimas)
                .HasForeignKey(d => d.LoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumo_Lote");

            entity.HasOne(d => d.MateriaPrima).WithMany(p => p.ConsumoMateriaPrimas)
                .HasForeignKey(d => d.MateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumo_MateriaPrima");

            entity.HasOne(d => d.Produccion).WithMany(p => p.ConsumoMateriaPrimas)
                .HasForeignKey(d => d.ProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumo_Produccion");
        });

        modelBuilder.Entity<HistorialPrueba>(entity =>
        {
            entity.HasKey(e => e.Idhistorial).HasName("PK__Historia__C4BEFB6951F38D51");

            entity.ToTable("HistorialPrueba");

            entity.Property(e => e.Idhistorial).HasColumnName("IDHistorial");
            entity.Property(e => e.FechaPrueba)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Idprueba).HasColumnName("IDPrueba");
            entity.Property(e => e.SerieId).HasColumnName("SerieID");
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.ValorMedido).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdpruebaNavigation).WithMany(p => p.HistorialPruebas)
                .HasForeignKey(d => d.Idprueba)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__IDPru__31B762FC");

            entity.HasOne(d => d.Serie).WithMany(p => p.HistorialPruebas)
                .HasForeignKey(d => d.SerieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Serie__30C33EC3");

            entity.HasOne(d => d.Usuario).WithMany(p => p.HistorialPruebas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__Historial__Usuar__32AB8735");
        });

        modelBuilder.Entity<LoteMateriaPrima>(entity =>
        {
            entity.HasKey(e => e.LoteId).HasName("PK__LoteMate__E6EAE6F818268C7E");

            entity.ToTable("LoteMateriaPrima");

            entity.Property(e => e.LoteId).HasColumnName("LoteID");
            entity.Property(e => e.CantidadDisponible).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CantidadInicial).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MateriaPrimaId).HasColumnName("MateriaPrimaID");

            entity.HasOne(d => d.MateriaPrima).WithMany(p => p.LoteMateriaPrimas)
                .HasForeignKey(d => d.MateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoteMateriaPrima_MateriaPrima");
        });

        modelBuilder.Entity<MateriaPrima>(entity =>
        {
            entity.HasKey(e => e.MateriaPrimaId).HasName("PK__MateriaP__5C27621E096292E0");

            entity.ToTable("MateriaPrima");

            entity.Property(e => e.MateriaPrimaId).HasColumnName("MateriaPrimaID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.Idorden).HasName("PK__Orden__5CBBCAD745FDA26E");

            entity.ToTable("Orden");

            entity.Property(e => e.Idorden).HasColumnName("IDOrden");
            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente para produccion");
            entity.Property(e => e.FechaOrden).HasColumnType("datetime");
            entity.Property(e => e.Idcliente).HasColumnName("IDCliente");
            entity.Property(e => e.Idproducto).HasColumnName("IDProducto");
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.Idcliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Cliente");

            entity.HasOne(d => d.IdproductoNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.Idproducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_ProductoTerminado");
        });

        modelBuilder.Entity<Produccion>(entity =>
        {
            entity.HasKey(e => e.ProduccionId).HasName("PK__Producci__6632E91D2FFF1BEB");

            entity.ToTable("Produccion");

            entity.HasIndex(e => e.OrdenId, "IX_Produccion_OrdenID");

            entity.Property(e => e.ProduccionId).HasColumnName("ProduccionID");
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.Orden).WithMany(p => p.Produccions)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK_Produccion_Orden");

            entity.HasOne(d => d.Producto).WithMany(p => p.Produccions)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Produccion_Producto");
        });

        modelBuilder.Entity<ProductoTerminado>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83B39BE080");

            entity.ToTable("ProductoTerminado");

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Prueba>(entity =>
        {
            entity.HasKey(e => e.Idprueba).HasName("PK__Prueba__31EA9050DC42A6FA");

            entity.ToTable("Prueba");

            entity.Property(e => e.Idprueba).HasColumnName("IDPrueba");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MetodoPrueba)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PruebaProducto>(entity =>
        {
            entity.HasKey(e => e.IdpruebaProducto).HasName("PK__PruebaPr__687920EBD8041791");

            entity.ToTable("PruebaProducto");

            entity.Property(e => e.IdpruebaProducto).HasColumnName("IDPruebaProducto");
            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Idprueba).HasColumnName("IDPrueba");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Tolerancia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ValorEsperado).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdpruebaNavigation).WithMany(p => p.PruebaProductos)
                .HasForeignKey(d => d.Idprueba)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PruebaPro__IDPru__2CF2ADDF");

            entity.HasOne(d => d.Producto).WithMany(p => p.PruebaProductos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PruebaPro__Produ__2BFE89A6");
        });

        modelBuilder.Entity<RecetaProducto>(entity =>
        {
            entity.HasKey(e => e.RecetaId).HasName("PK__RecetaPr__03D077B88AEFDE49");

            entity.ToTable("RecetaProducto");

            entity.Property(e => e.RecetaId).HasColumnName("RecetaID");
            entity.Property(e => e.CantidadNecesaria).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MateriaPrimaId).HasColumnName("MateriaPrimaID");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.MateriaPrima).WithMany(p => p.RecetaProductos)
                .HasForeignKey(d => d.MateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecetaProducto_MateriaPrima");

            entity.HasOne(d => d.Producto).WithMany(p => p.RecetaProductos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecetaProducto_Producto");
        });

        modelBuilder.Entity<SerieProducto>(entity =>
        {
            entity.HasKey(e => e.SerieId).HasName("PK__SeriePro__5C82513EFA6E28C2");

            entity.ToTable("SerieProducto");

            entity.Property(e => e.SerieId).HasColumnName("SerieID");
            entity.Property(e => e.EstatusCalidad)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProduccionId).HasColumnName("ProduccionID");

            entity.HasOne(d => d.Produccion).WithMany(p => p.SerieProductos)
                .HasForeignKey(d => d.ProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SerieProducto_Produccion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
