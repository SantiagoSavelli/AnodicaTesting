using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class Perfil
    {
        [Key]
        public int PerfilID { get; set; }

        [Required(ErrorMessage = "El Codigo es obligatorio.")]
        [MaxLength(50)]
        public string PerfilCodigoAlcemar { get; set; }

        [Required(ErrorMessage = "La Linea es obligatoria.")]
        public short LineaRef { get; set; }
        [ForeignKey("LineaRef")]
        [ValidateNever]
        public Linea? Linea { get; set; }

        public short? UbicacionRef { get; set; }
        [ForeignKey("UbicacionRef")]
        [ValidateNever]
        public Ubicacion? Ubicacion { get; set; }

        public byte[]? ImagenPerfil { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El peso por metro es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El valor debe estar entre 0 y 100")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal PesoXmetro { get; set; }

        [Required(ErrorMessage = "La Longitud de las tiras es obligatoria.")]
        [Range(0, 100, ErrorMessage = "El valor debe estar entre 0 y 100")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal LongTiraMts { get; set; }

        [Range(0, 100, ErrorMessage = "El valor debe estar entre 0 y 100")]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal? PesoXtira { get; set; }

        [Required(ErrorMessage = "La Cantidad Tiras/Paquete es obligatoria.")]
        public byte CantTirasPaquete { get; set; }

        [Required]
        public bool ManejaStockPropio { get; set; }

        public ICollection<PerfilTratamiento>? Tratamientos { get; set; }
    }
}
