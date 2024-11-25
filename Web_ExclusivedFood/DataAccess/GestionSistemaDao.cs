using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Windows.Forms;
using Web_ExclusivedFood.Models.Categoria;
using Web_ExclusivedFood.Models.Producto;

namespace Web_ExclusivedFood.DataAccess
{
    public class GestionSistemaDao
    {
        private readonly string _conexionString;
        private readonly string _apiKey;
        private readonly string _bucketName;

        public GestionSistemaDao()
        {

            _conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;
            _apiKey = "AIzaSyBAg0fXg1292bWjd3LJ1gDUj6zCEZnaCuM";
            _bucketName = "carkexfirebase.appspot.com";
        }

        // registros
        public async Task<bool> InsertarProductoConDetalleAsync(Productos p)
        {

            try
            {
                string url_Imagen = await subir_Image_FirebaseAsync(p.postFileImage);

                using (var connection = new SqlConnection(_conexionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_InsertarProductoConDetalle", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros
                        command.Parameters.AddWithValue("@idUsuario", ObtenerIdUsuarioDesdeCookie());
                        command.Parameters.AddWithValue("@nombre", p.Nombre);
                        command.Parameters.AddWithValue("@descripcion", p.Descripcion);
                        command.Parameters.AddWithValue("@precio", p.Precio);
                        command.Parameters.AddWithValue("@stock", p.Stock);
                        command.Parameters.AddWithValue("@id_categoria", p.IDCategoria);
                        command.Parameters.AddWithValue("@url_imagen", url_Imagen ?? ""); // Manejar nulos
                        command.Parameters.AddWithValue("@descripcion_detallada", p.Descripcion_detallada ?? "");
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

      


        //Modificacion de registros

        public async Task<bool> ActualizarProductoConDetalleAsync(Productos p)
        {

            // Subir imagen a Firebase  

            string url_Imagen = await subir_Image_FirebaseAsync(p.postFileImage);

            using (var connection = new SqlConnection(_conexionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ActualizarProductoConDetalle", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id_producto", p.IdProducto);
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@descripcion", p.Descripcion);
                    command.Parameters.AddWithValue("@precio", p.Precio);
                    command.Parameters.AddWithValue("@stock", p.Stock);
                    command.Parameters.AddWithValue("@id_categoria", p.IDCategoria);
                    command.Parameters.AddWithValue("@url_imagen", url_Imagen ?? p.url_imagen); // Manejar nulos en imagen
                    command.Parameters.AddWithValue("@descripcion_detallada", p.Descripcion_detallada ?? "");

                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
           
        }

        //vista categoria

        public async Task<bool> RegistrarCategoriaAsync(Categorias c)
        {
            string url_Imagen = await subir_Image_FirebaseAsync(c.postFileImage);

            try
            {
                using (SqlConnection connection = new SqlConnection(_conexionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_RegistrarCategoria", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@nombre", c.Nombre);
                        command.Parameters.AddWithValue("@url_imagen", url_Imagen ?? "");

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> EditarRegistroCategoriaAsync(Categorias c)
        {
            string url_Imagen = await subir_Image_FirebaseAsync(c.postFileImage);

            try
            {
                using (SqlConnection connection = new SqlConnection(_conexionString))
                {

                    using (SqlCommand command = new SqlCommand("sp_ActualizarCategoria", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@id_categoria", c.IdCategoria);
                        command.Parameters.AddWithValue("@nombre", c.Nombre);
                        command.Parameters.AddWithValue("@url_imagen", url_Imagen ?? c.UrlImagen);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }


        public async Task<string> subir_Image_FirebaseAsync(HttpPostedFileBase url_imagen)
        {
            try
            {
                if (url_imagen != null && url_imagen.ContentLength > 0)
                {

                    var storage = new FirebaseStorage(_bucketName, new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(_apiKey) //api key firebase
                    });

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(url_imagen.FileName);

                    using (var stream = url_imagen.InputStream)
                    {
                        var task = storage
                            .Child("Fotos_perfil/" + fileName)
                            .PutAsync(stream);

                        var url_Image = await task;

                        return url_Image;
                    }

                }
            }
            catch (Exception)
            {
            }
            return null;
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
