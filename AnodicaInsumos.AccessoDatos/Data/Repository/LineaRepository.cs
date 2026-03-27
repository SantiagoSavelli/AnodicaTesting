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

        public void Update(Linea linea)
        {
            var objDesdeDb = _db.Linea.FirstOrDefault(s => s.LineaID == linea.LineaID);
            if (objDesdeDb == null) return;
            
            objDesdeDb.LineaNombre = linea.LineaNombre;
            objDesdeDb.ProveedorRef = linea.ProveedorRef;
            objDesdeDb.LineaGrupoRef = linea.LineaGrupoRef;
        }
    }
}
