using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using AnodicaInsumos.Modelos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class PerfilRepository : Repository<Perfil, int>, IPerfilRepository 
    {
        private readonly ApplicationDbContext _db;
        public PerfilRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(Perfil perfil)
        {
            var objDesdeDb = _db.Perfil.FirstOrDefault(s => s.PerfilID == perfil.PerfilID);
            if (objDesdeDb == null) return;

            objDesdeDb.PerfilCodigoAlcemar = perfil.PerfilCodigoAlcemar;
            objDesdeDb.LineaRef = perfil.LineaRef;
            objDesdeDb.UbicacionRef = perfil.UbicacionRef;
            objDesdeDb.ImagenPerfil = perfil.ImagenPerfil;
            objDesdeDb.Descripcion = perfil.Descripcion;
            objDesdeDb.PesoXmetro = perfil.PesoXmetro;
            objDesdeDb.LongTiraMts = perfil.LongTiraMts;
            objDesdeDb.PesoXtira = perfil.PesoXtira;
            objDesdeDb.CantTirasPaquete = perfil.CantTirasPaquete;
            objDesdeDb.ManejaStockPropio = perfil.ManejaStockPropio;
        }
    }
}
