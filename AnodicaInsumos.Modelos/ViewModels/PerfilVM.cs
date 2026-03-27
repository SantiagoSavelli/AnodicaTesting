using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AnodicaInsumos.Modelos.ViewModels
{
    public class PerfilVM
    {
        public Perfil Perfil { get; set; } = new Perfil();

        public IFormFile? ArchivoImagen { get; set; }
        public bool EliminarImagen { get; set; } = true;

        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> Lineas { get; set; } = new();
        public List<SelectListItem> Ubicaciones { get; set; } = new();
        public List<SelectListItem> Tratamientos { get; set; } = new();

        public List<PerfilTratamientoVM> PerfilTratamientos { get; set; } = new();
        public List<PerfilEquivalenciaVM> PerfilEquivalencias { get; set; } = new();

        public bool SoloLectura { get; set; } = false;
    }
    public class PerfilTratamientoVM 
    {
        public short TratamientoRef { get; set; }
        public short CantMinimaTirasStock { get; set; }
        public short CantidadStock { get; set; }
        public short? UbicacionRef { get; set; }
        public string Descripcion { get; set; } = "";
    }

    public class PerfilEquivalenciaVM
    {
        public int? PerfilEquivalenteRef { get; set; }
        public string? Codigo { get; set; } 
        public string? Descripcion { get; set; }
    }
}