using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_ExclusivedFood.DataAccess;
using Web_ExclusivedFood.Models.Producto;
using Web_ExclusiveFood.DataAccess;

namespace Web_ExclusivedFood.Controllers.Users
{
    public class ReseñasController : Controller
    {
        ProductosDAO _producto = new ProductosDAO();
        ReseñaDao _reseña = new ReseñaDao();

        [HttpGet]
        public async Task<ActionResult> RealizarReseña()
        {
            var ID_Transaccion = TempData["IdTransaccion"] as int?;
            var infoPedido = TempData["DetallePedido"] as PedidoProducto;

            if (infoPedido == null || ID_Transaccion == null)
            {
                return RedirectToAction("Index","Home");
            }

            var infoTransaccion = await _producto.BuscarTransaccionPorIDAsync(ID_Transaccion);
            ViewBag.Transaccion = ID_Transaccion;   
            ViewBag.UrlImagen = infoPedido.url_imagen;
            ViewBag.NombreProducto = infoPedido.Nombre;

            return View(infoTransaccion);
        }

        [HttpPost]
        public async Task<ActionResult> RealizarReseña(Reseña reseña)
        {
            // Se realiza una operacion asincrona
            await _reseña.RegistrarReseñaAsync(reseña);

            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<ActionResult> Reseñas()
        {
            var Reseñas = await _reseña.ObtenerReseñasPorUsuarioAsync(); 
            return View(Reseñas);
        }
    }
}