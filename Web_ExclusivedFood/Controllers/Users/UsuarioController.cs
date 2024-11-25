using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_ExclusivedFood.DataAccess;
using Web_ExclusivedFood.Models.Usuario;

namespace Web_ExclusivedFood.Controllers.Users
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        private UsuarioDAO usuario = new UsuarioDAO();
        public async Task<ActionResult> Usuario()
        {
            var usu = await usuario.ObtenerUsuarioPorIDAsync();

            if (usu == null)
            {
                return RedirectToAction("CerrarSesion", "Login");
            }

            return View(usu);
        }

        public async Task<ActionResult> Editar()
        {
            var usu = await usuario.ObtenerUsuarioPorIDAsync();

            if (usu == null)
            {
                return RedirectToAction("CerrarSesion", "Login");
            }

            return View(usu);
        }

        public async Task<ActionResult> EditarUsuario(Usuario u)
        {
            var operacion = await usuario.ActualizarUsuarioAsync(u);

            if(operacion)
            {
                TempData["Mensaje"] = "Se actualizo con éxito.";

                return RedirectToAction("Usuario", "Usuario");
            }

            return RedirectToAction("Usuario", "Usuario");
        }

    }
}