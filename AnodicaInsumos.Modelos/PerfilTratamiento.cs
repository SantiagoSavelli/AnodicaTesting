using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    public class PerfilTratamiento
    {
        [Key]
        public int PerfilTratamientoId { get; set; }

        [Required(ErrorMessage = "Perfil obligatorio")]
        public int PerfilRef { get; set; }

        [Required(ErrorMessage = "Tratamiento obligatorio")]
        public short TratamientoRef { get; set; }

        [Required(ErrorMessage = "Cantidad de tiras obligatoria")]
        public short CantMinTirasStock { get; set; }

        [Required(ErrorMessage = "Cantidad de stock obligatorio")]
        public short CantStock { get; set; }

        public short UbicacionRef { get; set; }

        [ForeignKey("PerfilRef")]
        public Perfil? Perfil { get; set; }
        
        [ForeignKey("TratamientoRef")]
        public Tratamiento? Tratamiento { get; set; }

        [ForeignKey("UbicacionRef")]
        public Ubicacion? Ubicacion { get; set; }
    }
}
