using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class LineaGrupo
    {
        [Key]
        public byte LineaGrupoID { get; set; }
        
        [Required(ErrorMessage = "El grupo de linea es obligatorio")]
        public string LineaGrupoNombre { get; set; } = string.Empty;
    }
}
