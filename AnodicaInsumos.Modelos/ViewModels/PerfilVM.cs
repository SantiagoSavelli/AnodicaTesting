using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public bool SoloLectura { get; set; } = false;
    }
}