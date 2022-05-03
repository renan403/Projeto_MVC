using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;

using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {   

        [HttpPost]
        [HttpGet]
        public async Task<ActionResult> Login()
        {
           
            return View();
        }


        
        public async Task<ActionResult> Teste()
        {
            return View();
        }
        

        
        [HttpGet]
        public IActionResult CriarConta()
        {         
            return View(); 
        }
        [HttpPost]
        public IActionResult CriarConta(Usuario model)
        {
            if(model.ConfSenha != model.Senha){ ModelState.AddModelError("ConfSenha", "Senhas estão diferentes");}

            if (ModelState.IsValid)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(model); 
        }
        
        
        public IActionResult Carrinho()
        {
            return View();
        }
        public IActionResult ErrorLogin()
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
