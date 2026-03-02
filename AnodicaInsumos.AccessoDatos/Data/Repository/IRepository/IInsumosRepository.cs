using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IInsumosRepository : IRepository<Insumo, short>
    {
        void Update(Insumo insumo);

        IEnumerable<SelectListItem> GetListaInsumos();
    }
}
