using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Web_ExclusivedFood.DataAccess;
using Web_ExclusivedFood.Models.Categoria;
using Web_ExclusivedFood.Models.Home;
using Web_ExclusivedFood.Models.Producto;
using Web_ExclusiveFood.DataAccess;


namespace Web_ExclusivedFood.Controllers
{
    public class HomeController : Controller
    {
        private ProductosDAO _productoRepo = new ProductosDAO();
        private CategoriaDAO _categoriaRepo = new CategoriaDAO();
        private FavoritosDAO _favoritoRepo = new FavoritosDAO();


        public async Task<ActionResult> Index()
        {

            List<Productos> productos = await _productoRepo.ObtenerProductosAsync();

            Productos producto = await _productoRepo.ObtenerProductosRecienteAsync();

            List<Categorias> categoria = await _categoriaRepo.ObtenerCategoriasAsync();

            var viewModel = new MainModel
            {
                Categorias = categoria,
                Productos = productos,
                Producto = producto
            };

            return View(viewModel);
        }

        public ActionResult About()
        {
            return View();
        }
        public async Task<ActionResult> Favoritos()
        {
            var productos = await _favoritoRepo.ObtenerFavoritosPorUsuarioAsync();

            return View(productos);
        }

        public async Task<ActionResult> AgregarProductoFavorito(int idProducto)
        {
            var operacion = await _favoritoRepo.AgregarProductoAFavoritosAsync(idProducto);

            if (operacion)
            {
                TempData["Mensaje"] = "Producto Agregado a Deseos";
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> RemoverProductoFavorito(int IdProducto)
        {
            var operacion = await _favoritoRepo.EliminarProductoDeFavoritosAsync(IdProducto);

            if (operacion)
            {
                TempData["Mensaje"] = "Producto Eliminado de la lista de Deseos";
                return RedirectToAction("Favoritos", "Home");
            }
            return RedirectToAction("Favoritos", "Home");
        }


    }
}