using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class Insumo
    {
        [Key]
        public short InsumoID { get; set; }

        [Required(ErrorMessage = "Codigo de insumo necesario")]
        [MaxLength(50)]
        [Display(Name = "Codigo del Insumo")]
        public string CodigoInsumo { get; set; }

        [Required(ErrorMessage = "Nombre de insumo necesario")]
        [MaxLength(500)]
        [Display(Name = "Nombre del Insumo")]
        public string InsumoNombre { get; set; }

        [Required(ErrorMessage = "Unidad de medida es necesaria")]
        [MaxLength(5)]
        [Display(Name = "Unidad de medida")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "Cantidad de stock es necesaria")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Cantidad de stock")]
        public decimal CantidadStock { get; set; }

        [Required(ErrorMessage = "Cantidad minima de stock es necesaria")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Cantidad minima de stock")]
        public decimal CantMinimaStock { get; set; }
    }
}
