using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class Linea
    {
        [Key]
        public short LineaID { get; set; }

        [Required(ErrorMessage = "El nombre de la linea es obligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nombre de la linea")]
        public string LineaNombre { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int ProveedorRef { get; set; }

        public byte LineaGrupoRef { get; set; }

        [ForeignKey("ProveedorRef")]
        public Proveedor? Proveedor { get; set; }

        [ForeignKey("LineaGrupoRef")]
        public LineaGrupo? LineaGrupo { get; set; }

    }
}
