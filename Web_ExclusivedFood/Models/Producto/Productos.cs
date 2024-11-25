using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Producto
{
    public class Productos
    {

        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string url_imagen { get; set; } 
        public HttpPostedFileBase postFileImage { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IDCategoria { get; set; } 
        public string NombreCategoria { get; set; }
        public string Descripcion_detallada { get; set; }
        public string Descripcion_preparacion { get; set; }
        public string Tipo_mensaje { get; set; }
        public DetalleProducto DetalleProducto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioTotal { get; set; }
        public Reseña ReseñaProducto { get; set; }

    }
}