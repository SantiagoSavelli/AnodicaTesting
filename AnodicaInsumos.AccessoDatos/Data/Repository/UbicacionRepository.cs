using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class UbicacionRepository : Repository<Ubicacion, short>, IUbicacionRepository
    {
        private readonly ApplicationDbContext _db;
        public UbicacionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
