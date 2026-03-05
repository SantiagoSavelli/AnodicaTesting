using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnodicaInsumos.Modelos
{
    [Table("ProveedorTipoProveedor")]
    public class ProveedorTipoProveedor
    {
        [Key]
        public int ProveedorTipoProveedorID { get; set; }

        public int ProveedorRef { get; set; }
        public byte TipoProveedorRef { get; set; }

        [ForeignKey("ProveedorRef")]
        public Proveedor? Proveedor { get; set; }

        [ForeignKey("TipoProveedorRef")]
        public TipoProveedor? TipoProveedor { get; set; }
    }
}
