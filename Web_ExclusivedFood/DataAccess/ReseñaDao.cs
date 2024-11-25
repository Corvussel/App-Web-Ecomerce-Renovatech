using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Web_ExclusivedFood.Models.Producto;

namespace Web_ExclusivedFood.DataAccess
{
    public class ReseñaDao
    {

        private string _connectionString;

        public ReseñaDao()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;
        }

        public async Task RegistrarReseñaAsync(Reseña r)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("RegistrarResenaProducto", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //  parametros
                        cmd.Parameters.AddWithValue("@IdProducto", r.IdProducto);
                        cmd.Parameters.AddWithValue("@IdUsuario", r.IdUsuario);
                        cmd.Parameters.AddWithValue("@ValoracionEstrellas", r.Calificacion);
                        cmd.Parameters.AddWithValue("@TituloReseña", r.TituloReseña);
                        cmd.Parameters.AddWithValue("@Reseña", r.ReseñaProducto);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task<Reseña> ObtenerReseñasPorUsuarioAsync()
        {
            var idUsuario = ObtenerIdUsuarioDesdeCookie();
            Reseña Clasereseña = new Reseña();
            Clasereseña.ListaReseñas = new List<Reseña>();

            //relacionado a las valoraciones
            List<int> valoraciones = new List<int>();
            Dictionary<int, double> PorcentajePorValoracion = new Dictionary<int, double>();
            Dictionary<int, int> conteoPorValoracion = new Dictionary<int, int>()
            {
                {1,0}, {2,0}, {3,0}, {4,0}, {5,0}
            };


            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Sp_ObtenerReseñasPorUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) // Ejecuta la consulta de manera asincrónica
                        {
                            while (await reader.ReadAsync()) // Lee cada fila de manera asincrónica
                            {
                                var reseña = new Reseña()
                                {
                                    ReseñaId = reader.GetInt32(reader.GetOrdinal("reseña_id")),
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Calificacion = reader.GetInt32(reader.GetOrdinal("valoracion_estrellas")),
                                    TituloReseña = reader.GetString(reader.GetOrdinal("titulo_reseña")),
                                    ReseñaProducto = reader.GetString(reader.GetOrdinal("reseña")),
                                    NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                    UrlImagenUsuario = reader.GetString(reader.GetOrdinal("url_imagen_usuario")),
                                    NombreProducto = reader.GetString(reader.GetOrdinal("nombre_producto")),
                                    UrlImagenProducto = reader.GetString(reader.GetOrdinal("url_imagen_producto"))
                                };

                                Clasereseña.ListaReseñas.Add(reseña);

                                if (!reader.IsDBNull(reader.GetOrdinal("valoracion_estrellas")))
                                {
                                    valoraciones.Add(reader.GetInt32(reader.GetOrdinal("valoracion_estrellas")));
                                }
                            }

                        }
                    }
                }



                // de la lista de valoraciones se clasifica en base al numero de valoracion por tipo 1-5 estrellas
                foreach (var valor in valoraciones)
                {
                    if (conteoPorValoracion.ContainsKey(valor))
                    {
                        conteoPorValoracion[valor] += 1;
                    }
                }

                // se calcula el porcentaje que representa cada categoria de valoracion 1-5 estrellas

                foreach (var kvp in conteoPorValoracion)
                {
                    var porcentaje = valoraciones.Count() > 0 ? (kvp.Value / (double)valoraciones.Count()) * 100 : 0;

                    PorcentajePorValoracion.Add(kvp.Key, porcentaje);
                }


                // el promedio del total de valoraciones
                Clasereseña.valoracionPromedio = valoraciones.Any() ? (double)valoraciones.Sum() / valoraciones.Count() : 0;
                // el total de valoraciones
                Clasereseña.TotalValoracion = valoraciones.Count();
                // se le asigna a la clase reseña el valor del diccionario con los datos
                Clasereseña.ValoracionesPorcentajes = PorcentajePorValoracion;




            }
            catch (Exception e)
            {

                throw e;
            }

            return Clasereseña;
        }

        public async Task<Reseña> ObtenerReseñasProductoAsync(int? idProducto)
        { 
            Reseña Clasereseña = new Reseña();
            Clasereseña.ListaReseñas = new List<Reseña>();

            //relacionado a las valoraciones
            List<int> valoraciones = new List<int>();
            Dictionary<int, double> PorcentajePorValoracion = new Dictionary<int, double>();
            Dictionary<int, int> conteoPorValoracion = new Dictionary<int, int>()
            {
                {1,0}, {2,0}, {3,0}, {4,0}, {5,0}
            };


            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Sp_ObtenerReseñasPorProducto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_Producto", idProducto);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) // Ejecuta la consulta de manera asincrónica
                        {
                            while (await reader.ReadAsync()) // Lee cada fila de manera asincrónica
                            {
                                var reseña = new Reseña()
                                {
                                    ReseñaId = reader.GetInt32(reader.GetOrdinal("reseña_id")),
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Calificacion = reader.GetInt32(reader.GetOrdinal("valoracion_estrellas")),
                                    TituloReseña = reader.GetString(reader.GetOrdinal("titulo_reseña")),
                                    ReseñaProducto = reader.GetString(reader.GetOrdinal("reseña")),
                                    NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                    UrlImagenUsuario = reader.GetString(reader.GetOrdinal("url_imagen_usuario")),
                                    NombreProducto = reader.GetString(reader.GetOrdinal("nombre_producto")),
                                    UrlImagenProducto = reader.GetString(reader.GetOrdinal("url_imagen_producto"))
                                };

                                Clasereseña.ListaReseñas.Add(reseña);

                                if (!reader.IsDBNull(reader.GetOrdinal("valoracion_estrellas")))
                                {
                                    valoraciones.Add(reader.GetInt32(reader.GetOrdinal("valoracion_estrellas")));
                                }
                            }

                        }
                    }
                }



                // de la lista de valoraciones se clasifica en base al numero de valoracion por tipo 1-5 estrellas
                foreach (var valor in valoraciones)
                {
                    if (conteoPorValoracion.ContainsKey(valor))
                    {
                        conteoPorValoracion[valor] += 1;
                    }
                }

                // se calcula el porcentaje que representa cada categoria de valoracion 1-5 estrellas

                foreach (var kvp in conteoPorValoracion)
                {
                    var porcentaje = valoraciones.Count() > 0 ? (kvp.Value / (double)valoraciones.Count()) * 100 : 0;

                    PorcentajePorValoracion.Add(kvp.Key, porcentaje);
                }


                // el promedio del total de valoraciones
                Clasereseña.valoracionPromedio = valoraciones.Any() ? (double)valoraciones.Sum() / valoraciones.Count() : 0;
                // el total de valoraciones
                Clasereseña.TotalValoracion = valoraciones.Count();
                // se le asigna a la clase reseña el valor del diccionario con los datos
                Clasereseña.ValoracionesPorcentajes = PorcentajePorValoracion;  
            }
            catch (Exception e)
            {

                throw e;
            }

            return Clasereseña;
        }


        public int? ObtenerIdUsuarioDesdeCookie()
        {
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                var datos = ticket.UserData.Split(';');
                if (datos.Length > 1)
                {
                    return int.Parse(datos[1]);
                }
            }
            return null;
        }


    }
}