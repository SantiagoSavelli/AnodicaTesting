using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public class TipoProveedorRepository : Repository<TipoProveedor, byte>, ITipoProveedorRepository
    {
        private readonly ApplicationDbContext _db;

        public TipoProveedorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
