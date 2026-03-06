using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class Ubicacion
    {
        [Key]
        public short UbicacionID { get; set; }

        [Required(ErrorMessage = "El código de ubicación es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El código no puede superar los 100 caracteres.")]
        [Column(TypeName = "nchar(100)")]
        public string UbicacionCodigo { get; set; }

        [Required(ErrorMessage = "La descripción de la ubicación es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string UbicacionDesc { get; set; }
    }
}
