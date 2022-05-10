using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class ContaController : Controller
    {   
        [HttpGet]
        public IActionResult SuaConta()
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado");
            
            
            return View();
        }
    }
}
