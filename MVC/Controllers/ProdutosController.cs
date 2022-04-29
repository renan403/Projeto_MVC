using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class ProdutosController : Controller
    {
        public IActionResult Produtos()
        {
            return View();
        }
        public IActionResult ComprarProd()
        {
            return View();
        }
    }
}
