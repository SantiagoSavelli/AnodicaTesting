using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class LineaRepository : Repository<Linea, short>, ILineaRepository
    {
        private readonly ApplicationDbContext _db;
        public LineaRepository(ApplicationDbContext db) : base(db)
        { 
            _db = db;
        }
    }
}
