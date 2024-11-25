using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Web_ExclusivedFood.Models.Producto;
using System.Windows.Forms;

namespace Web_ExclusiveFood.DataAccess
{
    public class ProductosDAO
    {
        private readonly string _conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;


        public async Task<List<Productos>> ObtenerProductosAsync()
        {

            var productos = new List<Productos>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_MostrarProductos", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var producto = new Productos
                                {
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "Sin descripción" : reader.GetString(reader.GetOrdinal("descripcion")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                                    url_imagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                };

                                productos.Add(producto);
                            }
                        }
                        return productos;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public async Task<Productos> ObtenerProductosRecienteAsync()
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerUltimoProducto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var producto = new Productos
                                {
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "Sin descripción" : reader.GetString(reader.GetOrdinal("descripcion")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                                    url_imagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                };
                                return producto;
                            }

                        }

                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }


        public async Task<Productos> ObtenerProductoPorIdAsync(int? idProducto)
        {
            Productos producto = null;
            try
            {

                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_BuscarProductosPorID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_producto", idProducto);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                producto = new Productos
                                {
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre_producto")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                    url_imagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                    IDCategoria = reader.GetInt32(reader.GetOrdinal("id_categoria")),
                                };
                                return producto;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }


        public async Task<Productos> ObtenerProductoDetalleAsync(int? idProducto)
        {
            Productos producto = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("ObtenerProductoDetalle", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_producto", idProducto);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                producto = new Productos
                                {
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                                    url_imagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                    IDCategoria = reader.GetInt32(reader.GetOrdinal("id_categoria")),
                                    DetalleProducto = new DetalleProducto
                                    {
                                        IdDetalleProducto = reader.IsDBNull(reader.GetOrdinal("id_detalle_producto")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("id_detalle_producto")),
                                        DescripcionDetallada = reader.IsDBNull(reader.GetOrdinal("descripcion_detallada")) ? "Sin descripcion" : reader.GetString(reader.GetOrdinal("descripcion_detallada"))
                                    }
                                };

                                return producto;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public async Task<(bool Operacion, int IdTransaccion)> RegistrarTransaccionAsync(PedidoProducto p)
        {
            var idUsuario = ObtenerIdUsuarioDesdeCookie();
            int IdTransaccion = 0;

            if (idUsuario != null)
            {


                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("RegistrarTransaccion", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
                        cmd.Parameters.AddWithValue("@Cantidad", p.Cantidad);
                        cmd.Parameters.AddWithValue("@PrecioTotal", p.PrecioTotal);
                        cmd.Parameters.AddWithValue("@Direccion", p.Direccion);
                        cmd.Parameters.AddWithValue("@NumeroTelefonico", p.NumeroTelefonico);
                        cmd.Parameters.AddWithValue("@NombreTitular", p.NombreTitular);
                        cmd.Parameters.AddWithValue("@NumeroTarjeta", p.NumeroTarjeta);
                        cmd.Parameters.AddWithValue("@FechaExpiracion", p.FechaExpiracion);
                        cmd.Parameters.AddWithValue("@CVV", p.CVV);

                        object resultadoObj = await cmd.ExecuteScalarAsync();
                        int.TryParse(resultadoObj?.ToString(), out IdTransaccion);
                        return (true, IdTransaccion);
                    }
                }

            }
            else
            {
                return (false, IdTransaccion);
            }

        }

        public async Task<PedidoProducto> BuscarTransaccionPorIDAsync(int? idTransaccion)
        {
            PedidoProducto transaccion = null;
 
            try
            {
                using (SqlConnection connection = new SqlConnection(_conexionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("Sp_BuscarTransaccionPorID", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idTransaccion", idTransaccion);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                transaccion = new PedidoProducto
                                {
                                    IdTransaccion = reader.GetInt32(reader.GetOrdinal("id_transaccion")),
                                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                    FechaTransaccion = reader.GetDateTime(reader.GetOrdinal("fecha_transaccion"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return transaccion;
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