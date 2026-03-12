using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;

        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Insumo = new InsumosRepository(_db);
            Proveedor = new ProveedorRepository(_db);
            TipoProveedor = new TipoProveedorRepository(_db);
            ProveedorTipoProveedor = new ProveedorTipoProveedorRepository(_db);
            Linea = new LineaRepository(_db);
            LineaGrupo = new LineaGrupoRespository(_db);
            Perfil = new PerfilRepository(_db);
            PerfilEquivalencia = new PerfilEquivalenciaRepository(_db);
            Ubicacion = new UbicacionRepository(_db);
            PerfilTratamiento = new PerfilTratamientoRepository(_db);
            Tratamiento = new TratamientoRepository(_db);
            LineaGrupoTratamiento = new LineaGrupoTratamientoRepository(_db);
        }

        public IInsumosRepository Insumo {  get; private set; }
        public IProveedorRepository Proveedor { get; private set; }
        public ITipoProveedorRepository TipoProveedor { get; private set; }
        public IProveedorTipoProveedorRepository ProveedorTipoProveedor { get; private set; }
        public ILineaRepository Linea { get; private set; }
        public ILineaGrupoRepository LineaGrupo { get; private set; }
        public IPerfilRepository Perfil { get; private set; }
        public IPerfilEquivalenciaRepository PerfilEquivalencia { get; private set; }
        public IUbicacionRepository Ubicacion { get; private set; }
        public IPerfilTratamientoRepository PerfilTratamiento { get; private set; }
        public ITratamientoRepository Tratamiento { get; private set; }
        public ILineaGrupoTratamientoRepository LineaGrupoTratamiento { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        // Usar el save de modo asincronico para evitar bloqueos en la interfaz de usuario
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
