using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Reflection;
using System.Web.Security;
using System.Net;
using Web_ExclusivedFood.Models.Login;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Web_ExclusiveFood.DataAccess
{
    public class LoginDAO
    {
        private readonly string conexionString = ConfigurationManager.ConnectionStrings["conex"].ConnectionString;
        private readonly string _patron = "SISE2024";


        public async Task<(string mensaje, bool esValido)> IniciarSesionAsync(LoginViewModel usuario)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(conexionString))
                {
                    await conn.OpenAsync();

                    if (!await VerificarCuenta(conn, usuario.CorreoElectronico))
                    {
                        return ("La cuenta de usaurio no existe, Crea un nueva cuentan", false);
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_ValidarCredenciales", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@email", usuario.CorreoElectronico);
                        cmd.Parameters.AddWithValue("@contrasena", usuario.Password);
                        cmd.Parameters.AddWithValue("@patron", _patron);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int nuevoUsuarioId = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                                string nombreRol = reader.GetString(reader.GetOrdinal("nombre_rol"));

                                var roles = new string[] { nombreRol };
                                CookieAuthUsuario(usuario.CorreoElectronico, roles, nuevoUsuarioId);
                                return ("Bienvenido", true);
                            }
                            else
                            {
                                return ("La Contraseña es incorrecta, intentalo nuavamente", false);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                return ("Ocurrio un Error intanlo mas tarde", false);

            }
        }



        public async Task<(string mensaje, bool esValido)> RegistrarUsuarioAsync(LoginViewModel usuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexionString))
                {

                    await conn.OpenAsync();

                    if (await VerificarCuenta(conn, usuario.CorreoElectronico))
                    {
                        return ("La cuenta de usuario ya existe, Inicia Sesión", false);
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", usuario.NombreCompleto);
                        cmd.Parameters.AddWithValue("@email", usuario.CorreoElectronico);
                        cmd.Parameters.AddWithValue("@contrasena", usuario.Password);
                        cmd.Parameters.AddWithValue("@direccion", usuario.Direccion);
                        cmd.Parameters.AddWithValue("@telefono", usuario.Telefono);
                        cmd.Parameters.AddWithValue("@ciudad", usuario.Ciudad);
                        cmd.Parameters.AddWithValue("@codigo_postal", usuario.CodigoPostal);
                        cmd.Parameters.AddWithValue("@patron", _patron);


                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int nuevoUsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId"));
                                string nombreRol = reader.GetString(reader.GetOrdinal("NombreRol"));

                                var roles = new string[] { nombreRol };

                                //almacenaos informacion en la cookie de la sesion actual
                                CookieAuthUsuario(usuario.NombreCompleto, roles, nuevoUsuarioId);

                                return ("Bienvenido", true);
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return ("Ocurrió un Error al iniciar Sesión", false);
            }
            return ("", false);
        }


        private async Task<bool> VerificarCuenta(SqlConnection con, string email)
        {

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_VerificarCuenta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null && (int)result > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void CookieAuthUsuario(string nombreUsuario, string[] roles, int idUsuario)
        {
            try
            {

                var ticketAutenticacion = new FormsAuthenticationTicket(
                         1,
                         nombreUsuario,
                         DateTime.Now,
                         DateTime.Now.AddMonths(2),
                         true,
                         $"{string.Join(",", roles)};{idUsuario}"
                     );

                string ticketEncriptado = FormsAuthentication.Encrypt(ticketAutenticacion);

                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncriptado)
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMonths(2)// tiempo de  la expiración
                };

                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
            catch (Exception)
            {

            }
        }


        //cookie de autenticacion para almacenar informacion de la sesion
        private void CookieAuthUsuarios(string nombreUsuario, string[] roles)
        {
            try
            {

                var ticketAutenticacion = new FormsAuthenticationTicket(
                         1,
                         nombreUsuario,
                         DateTime.Now,
                         DateTime.Now.AddMonths(2),
                         true,
                         string.Join(",", roles)
                     );

                string ticketEncriptado = FormsAuthentication.Encrypt(ticketAutenticacion);

                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncriptado)
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMonths(2)// tiempo de  la expiración
                };

                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
            catch (Exception)
            {

            }
        }


    }
}