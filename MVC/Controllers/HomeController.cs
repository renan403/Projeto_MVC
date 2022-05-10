using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using System.Text;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {

        [HttpPost]
        [HttpGet]
        public IActionResult Home()
        {
            var sessaoNome = HttpContext.Session.GetString("SessaoNome");
            var sessaoEmail = HttpContext.Session.GetString("SessaoEmail");
            if(!string.IsNullOrEmpty(sessaoNome))
            {
                var nome = sessaoNome.Split(" ");
                if(nome.Length == 1) 
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);
                    ViewData["nomeUser"] = $"{primeiroNome}";
                    return View();
                }
                if(nome[1].Length <= 3)
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);
                    var conjunto = nome[1];
                    var sobreNome = char.ToUpper(nome[2][0]) + nome[2].Substring(1);
                    HttpContext.Session.SetString("nomeFormatado", $"{primeiroNome} {conjunto} {sobreNome}");
                    ViewData["nomeUser"] = $"{primeiroNome} {conjunto} {sobreNome}";
                }
                else
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);
                    var sobreNome = char.ToUpper(nome[1][0]) + nome[1].Substring(1);
                    HttpContext.Session.SetString("nomeFormatado", $"{primeiroNome} {sobreNome}");
                    ViewData["nomeUser"] = $"{primeiroNome} {sobreNome}";
                }               
            }
                       
            return View();
        }
        [HttpGet("/Home/Home/Sair")]
        public IActionResult HomeSair()
        {
            HttpContext.Session.SetString("SessaoNome", "");
            return RedirectToAction("Home", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}