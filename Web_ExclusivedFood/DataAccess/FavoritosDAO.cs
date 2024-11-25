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
    public class FavoritosDAO
    {
        private readonly string _conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;



        public async Task<List<Productos>> ObtenerFavoritosPorUsuarioAsync()
        {
            List<Productos> favoritos = new List<Productos>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_conexionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ObtenerFavoritosPorUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_usuario", ObtenerIdUsuarioDesdeCookie());

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var producto = new Productos
                                {
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                    url_imagen = reader.GetString(reader.GetOrdinal("url_imagen"))
                                };
                                favoritos.Add(producto);
                            }

                            return favoritos;
                        }
                    }
                }
            }
            catch (Exception)
            { 
            }
            return null;
        }

        public async Task<bool> AgregarProductoAFavoritosAsync(int idProducto)
        {
            using (SqlConnection connection = new SqlConnection(_conexionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("AgregarProductoAFavoritos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del SP
                    command.Parameters.AddWithValue("@id_usuario", ObtenerIdUsuarioDesdeCookie());
                    command.Parameters.AddWithValue("@id_producto", idProducto);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return true;  
                    }
                    catch (SqlException ex)
                    { 
                        Console.WriteLine($"Error al agregar el producto a favoritos: {ex.Message}");
                        return false;  
                    }
                }
            }
        }

        public async Task<bool> EliminarProductoDeFavoritosAsync(int idProducto)
        {
            using (SqlConnection connection = new SqlConnection(_conexionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("EliminarProductoDeFavoritos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                     
                    command.Parameters.AddWithValue("@id_usuario", ObtenerIdUsuarioDesdeCookie());
                    command.Parameters.AddWithValue("@id_producto", idProducto);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        return true;  
                    }
                    catch (SqlException ex)
                    { 
                        Console.WriteLine($"Error al eliminar el producto de favoritos: {ex.Message}");
                        return false;  
                    }
                }
            }
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