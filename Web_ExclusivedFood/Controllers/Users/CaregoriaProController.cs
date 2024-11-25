using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_ExclusivedFood.DataAccess;
using Web_ExclusivedFood.Models.Producto;

namespace Web_ExclusivedFood.Controllers.Users
{
    public class CaregoriaProController : Controller
    {
        // GET: CaregoriaPro
        private CategoriaDAO _categoriaRepo = new CategoriaDAO();


        public async Task<ActionResult> CategoriaProducto(int id)
        {
            List<Productos> productos = await _categoriaRepo.ProductosPorCategoriaIDAsync(id);

            return View(productos);
        }

    }
}