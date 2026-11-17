using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia
{

    public class BaseDbContext : DbContext
    {
        public BaseDbContext(
            DbContextOptions<BaseDbContext> options)
            : base(options)
        {
        }

        public DbSet<MenuEntidad> Menus => Set<MenuEntidad>();
        public DbSet<RolEntidad> Roles => Set<RolEntidad>();
        public DbSet<RolMenuEntidad> RolesMenu => Set<RolMenuEntidad>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuEntidad>(entity =>
            {
                entity.ToTable("Menus");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Url).HasMaxLength(250);
                entity.Property(x => x.Icono).HasMaxLength(50);
                entity.Property(x => x.Nivel).HasMaxLength(20);

                entity.HasOne(x => x.MenuPadre)
                    .WithMany(x => x.SubMenus)
                    .HasForeignKey(x => x.MenuPadreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RolEntidad>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            modelBuilder.Entity<RolMenuEntidad>(entity =>
            {
                entity.ToTable("RolesMenu");
                entity.HasKey(x => new { x.RolId, x.MenuId });

                entity.HasOne(x => x.Rol)
                    .WithMany(x => x.Menus)
                    .HasForeignKey(x => x.RolId);

                entity.HasOne(x => x.Menu)
                    .WithMany(x => x.RolesMenu)
                    .HasForeignKey(x => x.MenuId);
            });
        }
    }
}

