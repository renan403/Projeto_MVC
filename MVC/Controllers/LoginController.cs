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
        public ActionResult Login( string? Tela)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            var user = new ModelLogin();
            TempData["Tela"] = Tela;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(ModelLogin user)
        {

            var tela = TempData["Tela"] as string;
            string nome;
            string idUsuario;
            using (Auth auth = new())
                user.Resposta = await auth.LoginEmail(user.Email ?? "", user.Senha ?? "");

            if (user.Resposta == "Logged")
            {
                using (Data data = new())
                {
                    
                    idUsuario = await data.RetornaID(user.Email);
                    nome = await data.RetornaNome(idUsuario);
                    var end = await data.RetornaEnderecoPadrao(idUsuario);
                    if (end == null)
                    {
                        HttpContext.Session.SetString("Endereço", $"Cadastre");
                    }
                    else
                    {
                        HttpContext.Session.SetString("Endereço", $"{end.Object.Endereco}/{end.Object.Numero}/{end.Object.Cidade}/{end.Object.UF}/{end.Object.Cep}/{end.Object.Nome}");
                    }
                    var cartoes = await data.ReturnCard(idUsuario);
                    foreach(var cartao in cartoes)
                    {
                        HttpContext.Session.SetString("cartao", cartao.Key);
                        break;
                    }
                }
                HttpContext.Session.SetString("IdUsuario", idUsuario);
                
                HttpContext.Session.SetString("SessaoEmail", user.Email ?? "");
                HttpContext.Session.SetString("SessaoNome", nome);

                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SessaoNome")))// Verificar sessão para não dar erro nas condições
                {
                    using GeralService end = new();
                    HttpContext.Session.SetString("nomeFormatado", end.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
                }
                if(tela == null)
                     return RedirectToAction("Home", "Home");
                else
                {
                    var telaSep = tela.Split("/");
                    return RedirectToAction(telaSep[0], telaSep[1], new { id = telaSep[2] });
                }
                    
            }


            return View(user);

        }

        [HttpGet]
        public IActionResult CriarConta()
        {
            ModelUsuario model = new ();
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View(model);
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
                using (Data data = new())
                {

                    using Auth auth = new();
                    var rt = await auth.RegisterEmail(model.Email, model.Senha, model.Nome);
                    if (rt == "Criado")
                    {
                        await data.RegisterUser(model.Nome, model.Email);
                        TempData["email"] = model.Email;

                    }
                    else
                    {
                        model.Erro = rt;
                        return View(model);
                    }
                }

                return RedirectToAction("EmailEnviado", "Login");
            }
            return View(model);
        }
        public IActionResult EmailEnviado()
        {
            string email = TempData["email"] as string ?? "";
            if (email != "")
            {        
                ViewData["ContaEmailEnviado"] = TempData["email"];
                return View();
            }
            return RedirectToAction("Home", "Home");
        }        
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            ModelLogin model = new();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(ModelLogin model)
        {

            TempData["email"] = model.Email;
            if(TempData["email"] as string != "" || TempData["email"] != null) 
            {
                using Auth auth = new();
                var resp = await auth.ResetPassword(model.Email ?? "");
                if (resp == "Altered")
                {
                    return RedirectToAction("EmailEnviado", "Login");
                }
                model.Resposta = resp;
                return View(model);
            }
            return RedirectToAction("Home", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
