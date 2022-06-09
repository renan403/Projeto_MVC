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
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado") ?? "";
            
            var user = new ModelLogin();

            return View(user);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ModelLogin user)
        {

            string nome;        
                //user.Ativo = await data.LoginUser(user.Email, user.Senha);
                using (Auth auth = new())
                    user.Resposta = await auth.LoginEmail(user.Email ?? "", user.Senha ?? "");
           
            if (user.Resposta == "Logged")
            {
                using (Data data = new())
                {          
                    nome = await data.RetornaNome(user.Email, user.Senha);
                }
                //Retornar Endereço Padrao atualizado
                using (Data data = new())
                {                   
                    var end = await data.RetornaEnderecoPadrao(user.Email);
                    if (end == null)
                    {
                        HttpContext.Session.SetString("Endereço", $"Cadastre");
                    }
                    else
                    {               
                        HttpContext.Session.SetString("Endereço", $"{end.Object.Endereco}/{end.Object.Numero}/{end.Object.Cidade}/{end.Object.UF}/{end.Object.Cep}/{end.Object.Nome}");                      
                    }
                }

                HttpContext.Session.SetString("SessaoEmail", user.Email ?? "");
                HttpContext.Session.SetString("SessaoNome", nome);

                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SessaoNome")))// Verificar sessão para não dar erro nas condições
                {
                    using EnderecoService end = new();
                    HttpContext.Session.SetString("nomeFormatado", end.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
                }              
                return RedirectToAction("Home", "Home");
            }
            else if(user.Resposta == "INVALID_PASSWORD")
            {
                user.Resposta = "Senha inválida";
            }
            else if (user.Resposta == "EMAIL_NOT_FOUND")
            {
                user.Resposta = "Email não encontrado";
            }
            return View(user);

        }

        [HttpGet]
        public IActionResult CriarConta()
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado") ?? "";
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

                    using Auth auth = new();
                    var rt = await auth.RegisterEmail(model.Email, model.Senha, model.Nome);
                    if (rt == "Criado")
                    {
                        await data.RegisterUser(model.Nome, model.Email, model.Senha);
                        TempData["email"] = model.Email;
                    }
                    else
                    {
                        Console.WriteLine(rt);
                    }
                }
                return RedirectToAction("Home", "Home");
            }
            return View(model);
        }

        //[HttpGet]//Principal para nao haver duplicidade
        //public IActionResult EnviarEmail(ModelUsuario user, string message)
        //{
        //    ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado") ?? "";
        //    user.Message = "";
        //    if (TempData["Message"] != null)
        //    {
        //        user.Message = TempData["Message"].ToString() == "Código Reenviado com Sucesso!" ? "Código Reenviado com Sucesso! " : "Errado";
        //        if (true) { ModelState.AddModelError("Codigo", TempData["Message"].ToString() ?? ""); }
        //    }
        //    if (TempData["email"] != null)
        //    {
        //        string tempEmail = TempData["email"].ToString() ?? "";
        //        HttpContext.Session.SetString("SessaoEmail", tempEmail);
        //    }
        //    user.Email = HttpContext.Session.GetString("SessaoEmail");
        //    if (user.Email == null)
        //    {
        //        return RedirectToAction("CriarConta", "Login");
        //    }
        //    return View(user);
        //}

        //[HttpGet("Login/EnviarEmail/{id}")]// Reenviar codigo
        //public async Task<IActionResult> EnviarEmail(ModelUsuario user)
        //{
        //    ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado") ?? "";
        //    int con = 0;
        //    user.Email = HttpContext.Session.GetString("SessaoEmail");
        //    if (con == 0)
        //    {
        //        con += 1;
        //        using (Gerador gera = new ())
        //        {
        //            //var Codigo = gera.aleatorios();
        //            //var rt = Enviar.Enviar.NovoUser(user.Email, Codigo);
        //            //using (Data data = new ())
        //            //{
        //            //    await data.ReenviarCodigo(user.Email, Codigo);
        //            //}
        //            TempData["Message"] = "Código Reenviado com Sucesso!";
        //            return RedirectToAction("EnviarEmail", "Login", new { id = "" });
        //        }
        //    }
        //    else
        //    {
        //        return View(user);
        //    }
        //}

        //[HttpPost]//Parte de Envio
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EnviarEmail(ModelUsuario user, string nada, string nada1)
        //{
        //    //using (Data data = new ())
        //    //{
        //    //    var email = HttpContext.Session.GetString("SessaoEmail");
        //    //    if (email != null)
        //    //    {
        //    //       var resultado = await data.ValidaCod(user.Codigo, email);                  
        //    //        if (true == resultado)
        //    //        {
        //    //            HttpContext.Session.SetString("SessaoEmail", "");
        //    //            return RedirectToAction("Home", "Home");
        //    //        }
        //    //    }
        //    //}
        //    TempData["Message"] = "Código Errado";
        //    return RedirectToAction("EnviarEmail", "Login", new { id = "" });
        //}

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
