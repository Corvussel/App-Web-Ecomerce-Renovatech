using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Producto
{
    public class DetalleProducto
    {
        public int? IdDetalleProducto { get; set; }
        public string DescripcionDetallada { get; set; }
        public string DescripcionPreparacion { get; set; }
        public string TipoMensaje { get; set; }
    }
}