using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Categoria
{
    public class Categorias
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public HttpPostedFileBase postFileImage { get; set; }

    }
}