using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class LineaGrupoTratamientoRepository : Repository<LineaGrupoTratamiento, short>, ILineaGrupoTratamientoRepository
    {
        private readonly ApplicationDbContext _db;
        public LineaGrupoTratamientoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}