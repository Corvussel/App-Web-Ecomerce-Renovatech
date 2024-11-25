using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Usuario
{
    public class Usuario
    {
        public int Id { get; set; }  
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }  
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdRol { get; set; }
        public string UrlImagen { get; set; }

        public HttpPostedFileBase postFileImage { get; set; }

    }
}