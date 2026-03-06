using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class PerfilEquivalenciaRepository : Repository<PerfilEquivalencia, int>, IPerfilEquivalenciaRepository
    {
        private readonly ApplicationDbContext _db;
        public PerfilEquivalenciaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
