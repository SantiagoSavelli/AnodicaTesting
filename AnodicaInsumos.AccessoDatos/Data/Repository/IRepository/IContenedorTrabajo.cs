using AnodicaInsumos.Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        // Repositorios
        IInsumosRepository Insumo { get; }
        IProveedorRepository Proveedor { get; }
        ITipoProveedorRepository TipoProveedor { get; }
        IProveedorTipoProveedorRepository ProveedorTipoProveedor { get; }
        void Save();
    }
}
