using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class ProveedorRepository : Repository<Proveedor, int>, IProveedorRepository
    {
        private readonly ApplicationDbContext _db;
        public ProveedorRepository(ApplicationDbContext db) : base(db)
        { 
            _db = db;
        }

        public void Update(Proveedor proveedor)
        {
            var objDesdeDb = _db.Proveedor.FirstOrDefault(p => p.ProveedorID == proveedor.ProveedorID);

            if (objDesdeDb != null)
            {
                objDesdeDb.ProveedorNombre = proveedor.ProveedorNombre;
                objDesdeDb.Telefonos = proveedor.Telefonos;
                objDesdeDb.Email = proveedor.Email;
                objDesdeDb.Productos = proveedor.Productos;
                objDesdeDb.PorcentajePesoTiraPerfil = proveedor.PorcentajePesoTiraPerfil;
            }
        }
    }
}
