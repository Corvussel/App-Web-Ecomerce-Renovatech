using Firebase.Storage;
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

namespace Web_ExclusivedFood.Controllers.Admin
{
    public class GestionSistemaController : Controller
    {
        // GET: GestionSistema
        private GestionSistemaDao gestion = new GestionSistemaDao();
        private ProductosDAO productos = new ProductosDAO();
        private CategoriaDAO categorias = new CategoriaDAO();

        [HttpGet]
        public async Task<ActionResult> Main()
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

        [HttpGet]
        public ActionResult Categoria()
        {
            return PartialView("Categoria");
        }
         

        [HttpPost]
        public async Task<ActionResult> RegistrarCategoria(Categorias categoria)
        {
            if (ModelState.IsValid)
            {
                var validacion = await gestion.RegistrarCategoriaAsync(categoria);

                if (validacion)
                {
                    TempData["MensajeRgt"] = "Categoria Registrado Exitosamente";
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

            return RedirectToAction("Main", "GestionSistema");
        }


        [HttpGet]
        public async Task<ActionResult> EditarCategoria(int id)
        {
            var categoria = await categorias.ObtenerCategoriasIDAsync(id);

            if (categoria == null)
            {
                return RedirectToAction("Main", "GestionSistema");
            }
            return View("EditarCategoria", categoria);
        }

        
        [HttpPost]
        public async Task<ActionResult> EditarCategoria(Categorias categoria)
        {
            var validacion = await gestion.EditarRegistroCategoriaAsync(categoria);

            if (validacion)
            {
                TempData["MensajeRgt"] = "Categoria Actualizado Exitosamente";

                return RedirectToAction("Main", "GestionSistema");
            }
            else
            {
                ViewBag.MensajeValidacion = "Ocurrio un error";
            } 

            return RedirectToAction("EditarCategoria", "GestionSistema",categoria);
        }




    }
}