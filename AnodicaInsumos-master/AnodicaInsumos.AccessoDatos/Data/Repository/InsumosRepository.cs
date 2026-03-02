using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class InsumosRepository : Repository<Insumo, short>, IInsumosRepository
    {
        private readonly ApplicationDbContext _db;

        public InsumosRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaInsumos()
        {
            return _db.Insumos.Select(c => new SelectListItem
            {
                Text = c.InsumoNombre,
                Value = c.InsumoID.ToString()
            }).ToList();
        }

        public void Update(Insumo insumo)
        {
            var objDesdeDb = _db.Insumos.FirstOrDefault(s => s.InsumoID  == insumo.InsumoID);
            if (objDesdeDb == null) return;

            objDesdeDb.CodigoInsumo = insumo.CodigoInsumo;
            objDesdeDb.InsumoNombre = insumo.InsumoNombre;
            objDesdeDb.UnidadMedida = insumo.UnidadMedida;
            objDesdeDb.CantidadStock = insumo.CantidadStock;
            objDesdeDb.CantMinimaStock = insumo.CantMinimaStock;
        }
    }
}
