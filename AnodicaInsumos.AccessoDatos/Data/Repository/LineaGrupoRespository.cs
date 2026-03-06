using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class LineaGrupoRespository : Repository<LineaGrupo, byte>, ILineaGrupoRepository
    {
        private readonly ApplicationDbContext _db;
        public LineaGrupoRespository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
