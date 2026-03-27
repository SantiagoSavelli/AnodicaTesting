using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface ILineaGrupoRepository : IRepository<LineaGrupo, byte>
    {
        void Update(LineaGrupo lineaGrupo);
    }
}
