using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_ExclusivedFood.Models.Producto
{
    public class Reseña
    {

        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public int Calificacion { get; set; }
        public double valoracionPromedio { get; set; }
        public int TotalValoracion { get; set; }
        public Dictionary<int, double> ValoracionesPorcentajes { get; set; }
        public string TituloReseña { get; set; }
        public string ReseñaProducto { get; set; }
        public int ReseñaId { get; set; }
        public string NombreUsuario { get; set; }
        public string UrlImagenUsuario { get; set; }
        public string NombreProducto { get; set; }
        public string UrlImagenProducto { get; set; }
        public List<Reseña> ListaReseñas { get; set; }


    }
}