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
    public class ProductoController : Controller
    {

        private ProductosDAO _producto = new ProductosDAO(); 
        private ReseñaDao _reseña = new ReseñaDao(); 


        [HttpGet]
        public ActionResult ProductoPestaña()
        {
            return View();
        }

        public async Task<ActionResult> DetallesProducto(int? id)
        {

            Productos producto = await _producto.ObtenerProductoDetalleAsync(id);
            Reseña reseña = await _reseña.ObtenerReseñasProductoAsync(id);
            producto.ReseñaProducto = reseña;

            if (producto == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("ProductoPestaña", producto);
        }


        [Authorize]
        public async Task<ActionResult> CarritoCompras(int? id)
        {
            var producto = await _producto.ObtenerProductoPorIdAsync(id);

            if (producto == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("CarritoCompras", producto);
        }


        [Authorize]
        public ActionResult CompraPaso1(PedidoProducto pedidoProducto)
        {
            if (pedidoProducto == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("CompraPaso1", pedidoProducto);
        }

        [Authorize]
        public ActionResult CompraPaso2(PedidoProducto pedidoProducto)
        {
            if (pedidoProducto == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("CompraPaso2", pedidoProducto);
        }

        [Authorize]
        public async Task<ActionResult> RealizarCompra(PedidoProducto pedidoProducto)
        {
            var resultado = await _producto.RegistrarTransaccionAsync(pedidoProducto);

            //se verifica si la operacion se realizo con exito
            if (resultado.Operacion)
            {
                TempData["Mensaje"] = "La transacción se realizó con éxito." ;

                //alamcenamos informacion temporal
                TempData["DetallePedido"] = pedidoProducto;
                TempData["IdTransaccion"] = resultado.IdTransaccion;

                //redireccionar a la vista reseñas junto al valor obtenido de la transaccion 
                return RedirectToAction("RealizarReseña", "Reseñas");
            }

            return View("CompraPaso2", pedidoProducto);
        }


    }
}