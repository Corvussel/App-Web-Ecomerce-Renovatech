using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_ExclusivedFood.DataAccess;
using Web_ExclusivedFood.Models.Categoria;
using Web_ExclusivedFood.Models.Home;
using Web_ExclusivedFood.Models.Producto;
using Web_ExclusiveFood.DataAccess;
using static System.Collections.Specialized.BitVector32;

namespace Web_ExclusivedFood.Controllers.Users
{
    public class GestionProductoController : Controller
    {
        // GET: GestionProducto
        private ProductosDAO productos = new ProductosDAO();
        private CategoriaDAO categorias = new CategoriaDAO();
        private GestionSistemaDao gestion = new GestionSistemaDao();

      

        [HttpGet]
        public async Task<ActionResult> Producto()
        {
            var categoria = await categorias.ObtenerCategoriasAsync();
            var producto = await productos.ObtenerProductosAsync();

            var mainModels = new MainModel
            {
                Categorias = categoria,
                Productos = producto

            };
            return View(mainModels);
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarProducto(Productos producto)
        {
            if (ModelState.IsValid)
            {
                var validacion = await gestion.InsertarProductoConDetalleAsync(producto);

                if (validacion)
                {
                    TempData["MensajeRgt"] = "Producto Registrado Exitosamente";
                }
                else
                {
                    ViewBag.MensajeValidacion = "Ocurrio un error";
                }
            }
            else
            {
                ViewBag.MensajeValidacion = "Por favor ingresa datos Validos";
            }

            return RedirectToAction("Producto", "GestionProducto");
        }



        [HttpGet]
        public async Task<ActionResult> EditarProducto(int id)
        {
            var categoria = await categorias.ObtenerCategoriasAsync();
            var producto = await productos.ObtenerProductoDetalleAsync(id);

            var mainModels = new MainModel
            {
                Categorias = categoria,
                Producto = producto

            };

            if (producto == null)
            {
                return RedirectToAction("Producto", "GestionProducto");
            }

            return View("EditarProducto", mainModels);
        }


        [HttpPost]
        public async Task<ActionResult> EditarProducto(Productos productos)
        {
            var validacion = await gestion.ActualizarProductoConDetalleAsync(productos);

            if (validacion)
            {
                TempData["MensajeRgt"] = "Producto Actualizado Exitosamente";

                return RedirectToAction("Producto", "GestionProducto");
            }

            return RedirectToAction("Producto", "GestionProducto");
        }

    }
}