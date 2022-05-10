using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using MVC.Models.Service;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {

        
        [HttpGet]
       
        public  ActionResult Login()
        {
            var user = new ModelLogin();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ModelLogin user)
        {
            string nome;
            using (Data data = new())
            {
                user.Ativo = await data.LoginUser(user.Email, user.Senha);
                nome = await data.RetornaNome(user.Email, user.Senha);
            }
            if (user.Ativo == true)
            {
                HttpContext.Session.SetString("SessaoEmail", user.Email ?? "");
                HttpContext.Session.SetString("SessaoNome", nome);        
                return RedirectToAction("Home", "Home");
            }           
                return View(user);
            
        }

        [HttpGet]
        public IActionResult CriarConta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarConta(ModelUsuario model)
        {
            if (model.ConfSenha != model.Senha)
            {
                ModelState.AddModelError("ConfSenha", "Senhas estão diferentes");
            }
            if (ModelState.IsValid)
            {
                using (Data data = new ())
                {
                    using (Gerador gerador = new ())
                    {
                        var cod = gerador.aleatorios();
                        var rt = await Enviar.Enviar.NovoUser(model.Email, cod);
                        if (rt == "Sucesso")
                        {
                            await data.RegisterUser(model.Nome, model.Email, model.Senha, cod);
                            TempData["email"] = model.Email;
                        }
                        else
                        {
                            Console.WriteLine(rt);
                        }
                    }
                }
                return RedirectToAction("EnviarEmail", "Login");
            }
            return View(model);
        }

        [HttpGet]//Principal para nao haver duplicidade
        public IActionResult EnviarEmail(ModelUsuario user, string message)
        {
            user.Message = "";
            if (TempData["Message"] != null)
            {
                user.Message = TempData["Message"].ToString() == "Código Reenviado com Sucesso!" ? "Código Reenviado com Sucesso! " : "Errado";
                if (true) { ModelState.AddModelError("Codigo", TempData["Message"].ToString() ?? ""); }
            }
            if (TempData["email"] != null)
            {
                string tempEmail = TempData["email"].ToString() ?? "";
                HttpContext.Session.SetString("SessaoEmail", tempEmail);
            }
            user.Email = HttpContext.Session.GetString("SessaoEmail");
            if (user.Email == null)
            {
                return RedirectToAction("CriarConta", "Login");
            }
            return View(user);
        }

        [HttpGet("Login/EnviarEmail/{id}")]// Reenviar codigo
        public async Task<IActionResult> EnviarEmail(ModelUsuario user)
        {
            int con = 0;
            user.Email = HttpContext.Session.GetString("SessaoEmail");
            if (con == 0)
            {
                con += 1;
                using (Gerador gera = new ())
                {
                    var Codigo = gera.aleatorios();
                    var rt = Enviar.Enviar.NovoUser(user.Email, Codigo);
                    using (Data data = new ())
                    {
                        await data.ReenviarCodigo(user.Email, Codigo);
                    }
                    TempData["Message"] = "Código Reenviado com Sucesso!";
                    return RedirectToAction("EnviarEmail", "Login", new { id = "" });
                }
            }
            else
            {
                return View(user);
            }
        }

        [HttpPost]//Parte de Envio
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarEmail(ModelUsuario user, string nada, string nada1)
        {
            using (Data data = new ())
            {
                var email = HttpContext.Session.GetString("SessaoEmail");
                if (email != null)
                {
                   var resultado = await data.ValidaCod(user.Codigo, email);                  
                    if (true == resultado)
                    {
                        HttpContext.Session.SetString("SessaoEmail", "");
                        return RedirectToAction("Home", "Home");
                    }
                }
            }
            TempData["Message"] = "Código Errado";
            return RedirectToAction("EnviarEmail", "Login", new { id = "" });
        }

        public IActionResult Carrinho()
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
