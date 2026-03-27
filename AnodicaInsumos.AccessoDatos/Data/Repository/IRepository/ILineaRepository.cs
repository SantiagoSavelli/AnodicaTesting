using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface ILineaRepository : IRepository<Linea, short>
    {
        void Update(Linea linea);
    }
}
