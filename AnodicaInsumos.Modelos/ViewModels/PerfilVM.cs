using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AnodicaInsumos.Modelos.ViewModels
{
    public class PerfilVM
    {
        public Perfil Perfil { get; set; } = new Perfil();

        public IFormFile? ArchivoImagen { get; set; }

        public List<short> TratamientoIds { get; set; } = new();
        public List<short?> UbicacionesTratamiento { get; set; } = new();
        public List<decimal> StockMinimo { get; set; } = new();

        public List<string> EquivalenciasCodigo { get; set; } = new();
        public List<string> EquivalenciasDescripcion { get; set; } = new();

        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> Lineas { get; set; } = new();
        public List<SelectListItem> Ubicaciones { get; set; } = new();
        public List<SelectListItem> Tratamientos { get; set; } = new();

        public List<PerfilTratamiento>? PerfilTratamientos { get; set; }
        public List<dynamic>? PerfilEquivalencias { get; set; }

        public List<PerfilTratamientoVM> TratamientosV2 { get; set; } = new();
        public bool SoloLectura { get; set; } = false;
    }
    public class PerfilTratamientoVM 
    {
        //public int PerfilTratamientoId { get; set; }

        
        public short TratamientoRef { get; set; }

        
        public short CantMinimaTirasStock { get; set; }

        
        public short CantidadStock { get; set; }

        public short? UbicacionRef { get; set; }
        public string Descripcion { get; set; } = "";
    }
}