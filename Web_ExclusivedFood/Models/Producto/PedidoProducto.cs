using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Producto
{
    public class PedidoProducto
    {
        public int IdProducto { get; set; }
        public int IdTransaccion { get; set; }
        public int IdUsuario { get; set; } 
        public string Nombre { get; set; }
        public string url_imagen { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public double PrecioTotal { get; set; }
        public string Direccion { get; set; }
        public string NumeroTelefonico { get; set; }
        public string NombreTitular { get; set; }
        public string NumeroTarjeta { get; set; }
        public string FechaExpiracion { get; set; }
        public string CVV { get; set; }
        public DateTime FechaTransaccion { get; set; }
    }
}