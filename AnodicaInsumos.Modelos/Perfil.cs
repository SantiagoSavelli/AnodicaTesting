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

        [Required]
        [MaxLength(50)]
        public string PerfilCodigoAlcemar { get; set; }

        [Required]
        public short LineaRef { get; set; }
        [ForeignKey("LineaRef")]
        public Linea Linea { get; set; }

        public short UbicacionRef { get; set; }
        [ForeignKey("UbicacionRef")]
        public Ubicacion? Ubicacion { get; set; }

        public byte[]? ImagenPerfil { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal PesoXmetro { get; set; }

        [Required]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal LongTiraMts { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal? PesoXtira { get; set; }

        [Required]
        public byte CantTirasPaquete { get; set; }

        [Required]
        public bool ManejaStockPropio { get; set; }
    }
}
