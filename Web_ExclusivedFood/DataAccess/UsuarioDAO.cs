using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Web_ExclusivedFood.Models.Usuario;

namespace Web_ExclusivedFood.DataAccess
{
    public class UsuarioDAO
    {
        private readonly string _conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;
        private GestionSistemaDao _geestionSistemaDao;
        private string _patron;


        public UsuarioDAO()
        {
            _geestionSistemaDao = new GestionSistemaDao();
            _patron = "SISE2024";
        }

        public async Task<Usuario> ObtenerUsuarioPorIDAsync()
        {
            try
            {
                Usuario usuario = null;

                using (SqlConnection conn = new SqlConnection(_conexionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", ObtenerIdUsuarioDesdeCookie());

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                usuario = new Usuario
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")), 
                                    Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? " " : reader.GetString(reader.GetOrdinal("direccion")),
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? " " : reader.GetString(reader.GetOrdinal("telefono")),
                                    Ciudad = reader.IsDBNull(reader.GetOrdinal("ciudad")) ? " " : reader.GetString(reader.GetOrdinal("ciudad")),
                                    CodigoPostal = reader.IsDBNull(reader.GetOrdinal("codigo_postal")) ? " " : reader.GetString(reader.GetOrdinal("codigo_postal")),
                                    FechaRegistro = reader.GetDateTime(reader.GetOrdinal("fecha_registro")),
                                    IdRol = reader.IsDBNull(reader.GetOrdinal("id_rol")) ? 0 : reader.GetInt32(reader.GetOrdinal("id_rol")),
                                    UrlImagen = reader.IsDBNull(reader.GetOrdinal("url_imagen")) ? "" : reader.GetString(reader.GetOrdinal("url_imagen")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? " " : reader.GetString(reader.GetOrdinal("email")),
                                };
                                return usuario;
                            }
                        }
                    }
                }


            }
            catch (Exception e)
            {
                throw e; 
            }
            return null;
        }

       
        public async Task<bool> ActualizarUsuarioAsync(Usuario u)
        {
            var url_img = await _geestionSistemaDao.subir_Image_FirebaseAsync(u.postFileImage);

            using (SqlConnection connection = new SqlConnection(_conexionString))
            {
                using (SqlCommand command = new SqlCommand("ActualizarUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id_usuario", u.Id);
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@contrasena", u.ContrasenaHash);
                    command.Parameters.AddWithValue("@direccion", u.Direccion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@telefono", u.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ciudad", u.Ciudad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@codigo_postal", u.CodigoPostal ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@url_imagen", url_img ?? u.UrlImagen);
                    command.Parameters.AddWithValue("@patron", _patron);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
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