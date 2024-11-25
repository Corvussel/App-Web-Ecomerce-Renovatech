using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web_ExclusiveFood.Controllers
{
    public class ProductoController : Controller
    {
        private ProductosDAO _producto = new ProductosDAO();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductoPestaña()
        {
            return View();
        }

        public async Task<ActionResult> DetallesProducto(int id)
        {
            var producto = await _producto.ObtenerProductoDetalleAsync(id);

            return View("ProductoPestaña", producto);
        }


        [Authorize]
        public async Task<ActionResult> CarritoCompras(int id)
        {
            var producto = await _producto.ObtenerProductoPorIdAsync(id);
            return View("CarritoCompras", producto);
        }


        [Authorize(Roles = "Administrador")]
        public ActionResult CompraPaso1(PedidoProducto pedidoProducto)
        {
            return View("CompraPaso1", pedidoProducto);
        }


        public ActionResult CompraPaso2(PedidoProducto pedidoProducto)
        {
            return View("CompraPaso2", pedidoProducto);
        }

        public async Task<ActionResult> RealizarCompra(PedidoProducto pedidoProducto)
        {
            bool Operacion = await _producto.RegistrarTransaccionAsync(pedidoProducto);

            if (Operacion)
            {
                TempData["Mensaje"] = "La transacción se realizó con éxito.";
                return RedirectToAction("Index", "Home");
            }

            return View("CompraPaso2", pedidoProducto);
        }



    }
}