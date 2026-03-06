using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class TratamientoRepository : Repository<Tratamiento, short>, ITratamientoRepository
    {
        private readonly ApplicationDbContext _db;
        public TratamientoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
