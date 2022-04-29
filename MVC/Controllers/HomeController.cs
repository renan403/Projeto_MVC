using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using System.Text;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        ModelController model = new ModelController();
        public async Task<IActionResult> Home(string? pesquisa, string? cep1, string? cep2)
        {          
            
            return View();
        }
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}