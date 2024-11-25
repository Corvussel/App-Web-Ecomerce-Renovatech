using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web_ExclusivedFood.Models.Categoria;
using Web_ExclusivedFood.Models.Producto;

namespace Web_ExclusivedFood.Models.Home
{
    public class MainModel
    {
        public List<Recomendacion> Recomendaciones { get; set; }
        public List<Categorias> Categoria { get; set; }
        public List<Productos> Productos { get; set; }
        public Productos Producto { get; set; }
        public List<Categorias> Categorias { get; internal set; }
    }
}