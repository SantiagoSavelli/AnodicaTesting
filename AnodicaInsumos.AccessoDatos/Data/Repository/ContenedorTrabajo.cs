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
        }

        public IInsumosRepository Insumo {  get; private set; }
        public IProveedorRepository Proveedor { get; private set; }
        public ITipoProveedorRepository TipoProveedor { get; private set; }
        public IProveedorTipoProveedorRepository ProveedorTipoProveedor { get; private set; }  

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
