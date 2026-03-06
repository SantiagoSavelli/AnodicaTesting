using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    [Table("Proveedor")]
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(300, ErrorMessage = "El nombre no puede superar los 300 caracteres")]
        [Display(Name = "Nombre del Proveedor")]
        public string ProveedorNombre { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "El telefono no puede superar los 300 caracteres")]
        [Display(Name = "Telefono del Proveedor")]
        public string? Telefonos { get; set; }

        [MaxLength(100, ErrorMessage = "El email no puede superar los 100 caracteres")]
        [Display(Name = "Email del Proveedor")]
        public string? Email { get; set; }

        [MaxLength(300, ErrorMessage = "El producto no puede superar los 300 caracteres")]
        [Display(Name = "Productos del Proveedor")]
        public string? Productos { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Cantidad minima de stock")]
        public decimal? PorcentajePesoTiraPerfil { get; set; }

        public ICollection<ProveedorTipoProveedor>? ProveedorTipos { get; set; } = new List<ProveedorTipoProveedor>();
    }
}