using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using System.Text;
using MVC.Models.Service;
namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        
        public async Task<IActionResult> Home()
        {
            using (Auth email = new())
            {
                // await email.CadatrarEmail("renancporto94@gmail.com", "12345678","renan");
                //await email.LoginEmail("renancporto94@gmail.com", "123456");
                //await email.DeleteEmail("renancporto94@gmail.com", "12345678");
                await email.ChangePassword("renancporto94@gmail.com", "123456", "12345");
            }

            //using (Data data = new())
            //{
            //    await data.Teste();

            //}
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado") ?? "";

            using (EnderecoService end = new())
            ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            

            var sessaoEmail = HttpContext.Session.GetString("SessaoEmail");
            


            return View();
        }
        [HttpGet("/Home/Home/Sair")]
        public IActionResult HomeSair()
        {
            HttpContext.Session.SetString("SessaoNome", "");
            HttpContext.Session.SetString("Endereço", "" );
            HttpContext.Session.SetString("nomeFormatado", "");
            return RedirectToAction("Home", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}