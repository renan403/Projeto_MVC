using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Service;
using Newtonsoft.Json;

namespace MVC.Controllers
{
    public class ContaController : Controller
    {
        [HttpGet]
        public IActionResult SuaConta()
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado");

            using (EnderecoService end = new())
            {
                ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            return View();
        }

        public async Task<IActionResult> Endereco()
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado");

            using (EnderecoService end = new())
            {
                ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            using (Data data = new())
            {
                var retorno = await data.PuxarEndereco(HttpContext.Session.GetString("SessaoEmail") ?? "");
                if (retorno == null)
                {
                    ViewData["EnderecoCadastrado"] = "Não";
                    return View();
                }
                else
                {
                    ViewData["EnderecoCadastrado"] = "Sim";

                    return View(retorno);
                }
            }

        }
        [HttpGet("/SuaConta/AlterarEndereco/{Key}")]
        public async Task<IActionResult> AlterarEndereco(string Key)
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado");

            using (EnderecoService end = new())
            {
                ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            var opcao = Key.Split("&");
            if (opcao[1] == "Alterar")
            {
                TempData["KeyOpcaoEnd"] = opcao[0];
            }
            else
            {
                using (Data data = new Data())
                {
                    await data.DeleteEndereco(opcao[0] ?? "erro", HttpContext.Session.GetString("SessaoEmail"));


                    //Retornar Endereço Padrao atualizado
                    var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("SessaoEmail"));
                    if (end != null)
                    {
                        HttpContext.Session.SetString("Endereço", $"{end.Object.Endereco}/{end.Object.Numero}/{end.Object.Cidade}/{end.Object.UF}/{end.Object.Cep}/{end.Object.Nome}");
                    }
                    else
                    {
                        HttpContext.Session.SetString("Endereço", $"Sem endereço/");
                    }
                }

                return RedirectToAction("Endereco", "Conta");
            }

            return View();
        }
        [HttpPost("/SuaConta/AlterarEndereco/{Key}")]
        public async Task<IActionResult> AlterarEndereco(ModelEndereço model)
        {
            var Key = TempData["KeyOpcaoEnd"].ToString();
            if (Key != string.Empty)
            {
                if (Key == "Alterar")
                {
                    using (Data data = new())
                    {
                        await data.AlterarEndereco(Key, HttpContext.Session.GetString("SessaoEmail") ?? "", model.Pais, model.Nome, model.Telefone, model.Cep,
                                                   model.Endereco, model.Numero, model.Complemento, model.Bairro, model.Cidade, model.Estado, model.Padrao, model.UF);

                        //Retornar Endereço Padrao atualizado
                        var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("SessaoEmail"));
                        if (end == null)
                        {
                            HttpContext.Session.SetString("Endereço", $"Cadastre/");
                        }
                        else
                        {
                            HttpContext.Session.SetString("Endereço", $"{end.Object.Endereco}/{end.Object.Numero}/{end.Object.Cidade}/{end.Object.UF}/{end.Object.Cep}/{end.Object.Nome}");
                        }
                    }
                }

                return RedirectToAction("Endereco", "Conta");
            }
            return RedirectToAction("Endereco", "Conta");
        }


        [HttpGet]
        public IActionResult AddEndereco()
        {
            ViewData["nomeUser"] = HttpContext.Session.GetString("nomeFormatado");

            using (EnderecoService end = new())
            {
                ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
        
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEndereco(ModelEndereço model)
        {
            if (ModelState.IsValid)
            {
                using (Data data = new Data())
                {
                    var resultado = await data.SalvarEndereco(HttpContext.Session.GetString("SessaoEmail") ?? "", model.Pais ?? "", model.Nome ?? "", model.Telefone ?? "",
                                                                                                                  model.Cep ?? "", model.Endereco ?? "", model.Numero,
                                                                                                                  model.Complemento ?? "", model.Bairro ?? "", model.Cidade ?? "",
                                                                                                                  model.Estado ?? "", model.Padrao, model.UF ?? "");

                    //Retornar Endereço Padrao atualizado
                    var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("SessaoEmail"));
                    if (end == null)
                    {
                        HttpContext.Session.SetString("Endereço", $"Cadastre/");
                    }
                    else
                    {

                        HttpContext.Session.SetString("Endereço", $"{end.Object.Endereco}/{end.Object.Numero}/{end.Object.Cidade}/{end.Object.UF}/{end.Object.Cep}/{end.Object.Nome}");
                    }
                    return RedirectToAction("Endereco", "Conta");
                }
            }
            return View();
        }
        public IActionResult Reembolso()
        {

            return View();
        }
        public IActionResult AcessoSeg()
        {
            var model = new ModelLogin();
            model.Email = HttpContext.Session.GetString("SessaoEmail");
            model.Nome = HttpContext.Session.GetString("SessaoNome");

            return View(model);
        }
        public IActionResult Carteira()
        {
            return View();
        }
        public IActionResult Transacoes()
        {
            return View();
        }
        public IActionResult SeusPedidos()
        {
            return View();
        }
        public IActionResult DetalhePedido()
        {
            return View();
        }
        public IActionResult ExibirRecibo()
        {
            return View();
        }
        public IActionResult VendaNoApp()
        {
            return View();
        }
    }
}