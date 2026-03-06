using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class ProveedorTipoProveedorRepository : Repository<ProveedorTipoProveedor, int>, IProveedorTipoProveedorRepository
    {
        private readonly ApplicationDbContext _db;

        public ProveedorTipoProveedorRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
    }
}
