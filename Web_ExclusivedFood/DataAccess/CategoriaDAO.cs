using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web_ExclusivedFood.Models.Categoria;
using Web_ExclusivedFood.Models.Producto;

namespace Web_ExclusivedFood.DataAccess
{
    public class CategoriaDAO
    {
        private readonly string _conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;

        public async Task<List<Categorias>> ObtenerCategoriasAsync()
        {
            var categorias = new List<Categorias>();

            using (SqlConnection conn = new SqlConnection(_conexionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_ListarCategorias", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var categoria = new Categorias
                            {
                                IdCategoria = reader.GetInt32(reader.GetOrdinal("id_categoria")),
                                UrlImagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            };
                            categorias.Add(categoria);
                        }
                    }
                }
            }
            return categorias;
        }

        public async Task<List<Productos>> ProductosPorCategoriaIDAsync(int idCategoria)
        {
            var productos = new List<Productos>();

            using (SqlConnection conn = new SqlConnection(_conexionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_BuscarProductosPorCategoria", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_categoria", idCategoria);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var producto = new Productos
                            {
                                IdProducto = reader.GetInt32(reader.GetOrdinal("id_producto")),
                                Nombre = reader.GetString(reader.GetOrdinal("nombre_producto")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "sin descripcion" : reader.GetString(reader.GetOrdinal("descripcion")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("precio")),
                                url_imagen = reader.GetString(reader.GetOrdinal("url_imagen")),
                                Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                                IDCategoria = reader.GetInt32(reader.GetOrdinal("id_categoria")),
                                NombreCategoria = reader.GetString(reader.GetOrdinal("nombre_categoria"))
                            };
                            productos.Add(producto);
                        }
                    }
                }
            }
            return productos;
        }


        public async Task<Categorias> ObtenerCategoriasIDAsync(int id)
        {
            var categorias = new Categorias();

            using (SqlConnection conn = new SqlConnection(_conexionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_BuscarCategoriaID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_categoria", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categorias.IdCategoria = reader.GetInt32(reader.GetOrdinal("id_categoria"));
                            categorias.UrlImagen = reader.GetString(reader.GetOrdinal("url_imagen"));
                            categorias.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                            return categorias;
                        } 
                    }
                }
            }
            return null;
        }

    }
}