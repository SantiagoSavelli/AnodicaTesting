using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public short CantMinimaTirasStock { get; set; }

        [Required(ErrorMessage = "Cantidad de stock obligatorio")]
        public short CantidadStock { get; set; }

        public short? UbicacionRef { get; set; }

        [ForeignKey("PerfilRef")]
        [ValidateNever]
        public Perfil? Perfil { get; set; }
        
        [ForeignKey("TratamientoRef")]
        [ValidateNever]
        public Tratamiento? Tratamiento { get; set; }

        [ForeignKey("UbicacionRef")]
        [ValidateNever]
        public Ubicacion? Ubicacion { get; set; }
    }
}
