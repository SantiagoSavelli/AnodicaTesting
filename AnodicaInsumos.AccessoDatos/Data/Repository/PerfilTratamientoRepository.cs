using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class PerfilTratamientoRepository : Repository<PerfilTratamiento, int>, IPerfilTratamientoRepository
    {
        private readonly ApplicationDbContext _db;
        public PerfilTratamientoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
