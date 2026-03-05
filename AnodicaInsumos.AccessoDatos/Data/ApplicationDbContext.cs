using AnodicaInsumos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        
        }

        // DbSets
        public DbSet<Insumo> Insumo { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<TipoProveedor> TipoProveedor { get; set; }
        public DbSet<ProveedorTipoProveedor> ProveedorTipoProveedor { get; set; }

        // Configuración de relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProveedorTipoProveedor>()
                .HasOne(x => x.Proveedor)
                .WithMany(p => p.ProveedorTipos)
                .HasForeignKey(x => x.ProveedorRef);

            modelBuilder.Entity<ProveedorTipoProveedor>()
                .HasOne(x => x.TipoProveedor)
                .WithMany(t => t.ProveedorTipos)
                .HasForeignKey(x => x.TipoProveedorRef);
        }
    }
}
