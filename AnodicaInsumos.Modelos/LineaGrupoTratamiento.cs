using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class LineaGrupoTratamiento
    {
        [Key]
        public short LineaGrupoTratamientoID { get; set; }

        [Required]
        public byte LineaGrupoRef { get; set; }

        [Required]
        public short TratamientoRef { get; set; }

        [Column(TypeName = "money")]
        public double PrecioXkgAluminio { get; set; }

        [ForeignKey("LineaGrupoRef")]
        public LineaGrupo LineaGrupo { get; set; }

        [ForeignKey("TratamientoRef")]
        public Tratamiento Tratamiento { get; set; }
    }
}
