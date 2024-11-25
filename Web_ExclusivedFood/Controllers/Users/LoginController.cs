using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_ExclusivedFood.Models.Login;
using Web_ExclusiveFood.DataAccess;

namespace Web_ExclusivedFood.Controllers.Users
{
    public class LoginController : Controller
    {
        // GET: Login
        private LoginDAO _usuarioDAO = new LoginDAO();

        [HttpGet]
        public ActionResult CrearCuenta()
        {
            return View();
        }

        [HttpGet]
        public ActionResult IniciarSesion()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> IniciarSesion(LoginViewModel model)
        {
            // Validar usuario y obtener roles

            var operacion = await _usuarioDAO.IniciarSesionAsync(model);

            if (operacion.esValido)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Mensaje = operacion.mensaje;
            }

            // Si el modelo no es válido, devolver la vista
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> CrearCuenta(LoginViewModel model)
        {

            var operacion = await _usuarioDAO.RegistrarUsuarioAsync(model);

            if (operacion.esValido)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Mensaje = operacion.mensaje;
            }

            return View();
        }


        public ActionResult CerrarSesion()
        {
            // Cerrar la sesion de autenticación
            FormsAuthentication.SignOut();
            // Redirigir a la pagina principal
            return RedirectToAction("Index", "Home");
        }

    }
}