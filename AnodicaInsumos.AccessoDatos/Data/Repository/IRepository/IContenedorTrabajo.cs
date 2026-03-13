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
        ILineaGrupoRepository LineaGrupo { get; }
        ILineaRepository Linea { get; }
        IPerfilRepository Perfil { get; }
        IPerfilEquivalenciaRepository PerfilEquivalencia { get; }
        IUbicacionRepository Ubicacion { get; }
        IPerfilTratamientoRepository PerfilTratamiento { get; }
        ITratamientoRepository Tratamiento { get; }
        ILineaGrupoTratamientoRepository LineaGrupoTratamiento { get; }

        // Usar el save de modo asincronico para evitar bloqueos en la interfaz de usuario
        void Save();
        Task SaveAsync();
    }
}
