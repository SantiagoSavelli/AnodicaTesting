using AnodicaInsumos.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IProveedorRepository : IRepository<Proveedor, int>
    {
        void Update(Proveedor proveedor);
    }
}