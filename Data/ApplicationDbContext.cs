using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Models;

namespace Pedidos360.Data
// Contexto de base de datos principal que incluye las entidades del sistema y la integración con Identity para la gestión de usuarios
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Pedido> Pedidos { get; set; }
        public virtual DbSet<PedidoDetalle> PedidoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo de IdentityUser a Usuarios según Diagrama ER
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.Property(e => e.Id).HasColumnName("id_Usuario");
                entity.Property(e => e.PasswordHash).HasColumnName("Password");
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.id_Categoria).HasName("PK_Categorias");
                entity.Property(e => e.Nombre).HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.id_Producto).HasName("PK_Productos");
                entity.Property(e => e.Nombre).HasMaxLength(150);
                entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImpuestoPorc).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Activo).HasDefaultValue(true);

                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.id_Categoria)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.id_Cliente).HasName("PK_Clientes");
                entity.Property(e => e.Nombre).HasMaxLength(200);
                entity.Property(e => e.Cedula).HasMaxLength(20);
                entity.Property(e => e.Correo).HasMaxLength(200);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(500);

                entity.HasIndex(e => e.Cedula).IsUnique();
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.id_Pedido).HasName("PK_Pedidos");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Impuestos).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Estado).HasMaxLength(50).HasDefaultValue("Pendiente");
                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.HasOne(p => p.Cliente)
                      .WithMany(c => c.Pedidos)
                      .HasForeignKey(p => p.id_Cliente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(p => p.id_Usuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.HasKey(e => e.id_detalleP).HasName("PK_PedidoDetalles");
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Descuento).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Porcentaje_Imp).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Total_Linea).HasColumnType("decimal(18,2)");

                // Si se borra el pedido, se borran sus detalles
                entity.HasOne(d => d.Pedido)
                      .WithMany(p => p.Detalles)
                      .HasForeignKey(d => d.id_Pedido)
                      .OnDelete(DeleteBehavior.Cascade);

                // No se puede borrar un producto si tiene pedidos
                entity.HasOne(d => d.Producto)
                      .WithMany(p => p.PedidoDetalles)
                      .HasForeignKey(d => d.id_Producto)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
