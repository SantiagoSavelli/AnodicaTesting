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
        public DbSet<Linea> Linea { get; set; }
        public DbSet<LineaGrupo> LineaGrupo { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<PerfilEquivalencia> PerfilEquivalencia { get; set; }
        public DbSet<Ubicacion> Ubicacion { get; set; }
        public DbSet<Tratamiento> Tratamiento { get; set; }
        public DbSet<PerfilTratamiento> PerfilTratamiento { get; set; }
        public DbSet<LineaGrupoTratamiento> LineaGrupoTratamiento { get; set; }

        // Configuacion de onModelCreating para relaciones muchos a muchos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PerfilEquivalencia>()
                .HasOne(pe => pe.Perfil)
                .WithMany(p => p.Equivalencias)
                .HasForeignKey(pe => pe.PerfilRef)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PerfilEquivalencia>()
                .HasOne(pe => pe.PerfilEquivalente)
                .WithMany()
                .HasForeignKey(pe => pe.PerfilEquivalenteRef)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
