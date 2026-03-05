using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    [Table("TipoProveedor")]
    public class TipoProveedor
    {
        [Key]
        public byte TipoProveedorID { get; set; }

        [Required]
        [Column(TypeName = "varchar(200)")]
        public string TipoProveedorNombre { get; set; } = string.Empty;

        public ICollection<ProveedorTipoProveedor>? ProveedorTipos { get; set; } = new List<ProveedorTipoProveedor>();
    }
}
