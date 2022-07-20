using Firebase.Database;

using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Service;
using Newtonsoft.Json;
using System.Drawing;
using ZXing;
using ZXing.Common;

namespace MVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly IWebHostEnvironment _env;  
        public ContaController(IWebHostEnvironment env)
        {
            this._env = env;
        }
        [HttpGet]
        public IActionResult SuaConta()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            using (GeralService end = new())
            {
                ViewBag.RNCUC = end.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            return View();
        }
        public async Task<IActionResult> Endereco()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            using (Data data = new())
            {
                var retorno = await data.PuxarEndereco(HttpContext.Session.GetString("IdUsuario") ?? "");
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
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var opcao = Key.Split("&");
            if (opcao[1] == "Alterar")
            {
                TempData["KeyOpcaoEnd"] = opcao[0];
            }
            else if (opcao[1] == "Padrao")
            {
                using (Data data = new())
                    await data.MudarPadrao(HttpContext.Session.GetString("IdUsuario"), opcao[0]);
                return RedirectToAction("Endereco", "Conta");
            }
            else
            {
                using (Data data = new Data())
                {
                     await data.DeleteEndereco(opcao[0] ?? "erro", HttpContext.Session.GetString("IdUsuario"));


                    //Retornar Endereço Padrao atualizado
                     var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("IdUsuario"));
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
                        await data.AlterarEndereco(Key, HttpContext.Session.GetString("IdUsuario") ?? "", model.Pais, model.Nome, model.Telefone, model.Cep,
                        model.Endereco, model.Numero, model.Complemento, model.Bairro, model.Cidade, model.Estado, model.Padrao, model.UF);

                        //Retornar Endereço Padrao atualizado
                        var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("IdUsuario"));
                        if (end == null)
                        {
                            HttpContext.Session.SetString("Endereço", $"Cadastre");
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
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
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
                    
                      var resultado = await data.SalvarEndereco(HttpContext.Session.GetString("IdUsuario") ?? "", model);

                    //Retornar Endereço Padrao atualizado
                   var end = await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("IdUsuario"));
                    if (end == null)
                    {
                        HttpContext.Session.SetString("Endereço", $"Cadastre");
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
        public async Task<IActionResult> Carteira(string erro)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            ModelCartao model = new ();
            model.Erro = erro;      
            using ( Data data = new())
            {
                ViewBag.ArrayCartoes = await data.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
                ViewBag.CartaoPadrao = await data.RetornarCartaoPadrao(HttpContext.Session.GetString("IdUsuario"));
            } 

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Carteira(ModelCartao model, string btn)
        {
            if (!string.IsNullOrEmpty(btn)){
                var separador = btn.Split("%%");
                btn = separador[0];
                switch (btn)
                {
                    case "remove":
                        using (Data data = new())
                            await data.DeleteCard(HttpContext.Session.GetString("IdUsuario"), separador[1]);
                        break;
                    case "alterar":
                            using (Data data = new())
                            await data.AlterarCard(HttpContext.Session.GetString("IdUsuario"), separador[1],model.NomeCard, model.DataExpiracao);
                        break;
                }
            }


            if(!string.IsNullOrEmpty(model.Bandeira) || !string.IsNullOrEmpty(model.NumeroCard))
            {
                using (Card card = new())
                {
                    card.Bandeira = model.Bandeira;
                    card.Cartao = model.NumeroCard;
                    var valido = card.CartaoValido();
                    if (valido)
                    {
                        switch (model.Bandeira)
                        {
                            case "DinerClub":
                                model.CaminhoImgBandeira = "../img/Cartoes/bandeira/dinerclub.png";
                                model.CaminhoImgCartao = "../img/Cartoes/DinerClub.png";
                                break;
                            case "MasterCard":
                                model.CaminhoImgBandeira = "../img/Cartoes/bandeira/mastercard.png";
                                model.CaminhoImgCartao = "../img/Cartoes/mastercardBlack.png";
                                break;
                            case "Visa":
                                model.CaminhoImgBandeira = "../img/Cartoes/bandeira/Visa.png";
                                model.CaminhoImgCartao = "../img/Cartoes/Visa.png";
                                break;
                            case "Amex":
                                model.CaminhoImgBandeira = "../img/Cartoes/bandeira/Amex.png";
                                model.CaminhoImgCartao = "../img/Cartoes/AMEX_Preto.jpg";
                                break;
                        }
                        using (Data data = new())
                        {
                            var resp = await data.SalvarCard(HttpContext.Session.GetString("IdUsuario"), model);
                            if(resp.Contains("sucesso"))
                                return RedirectToAction("Carteira", "Conta");
                            else
                            {
                                return RedirectToAction("Carteira", "Conta", new { erro = resp });
                            }
                        }
                    }              
                    return RedirectToAction("Carteira", "Conta", new {erro = "Cartão inválido ⚠️" });
                }
            }
           
            return RedirectToAction("Carteira","Conta");
        }  
        public async Task<IActionResult> AlterarCartao(string card)
        {
            using(Data data = new())
            {
                await data.MudarPadraoCard(HttpContext.Session.GetString("IdUsuario"),card);
            }    
            return RedirectToAction("Carteira", "Conta");
        }
        public async Task<IActionResult> FinalizarCompra(string cardTrocar, string finalizar)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();

            using (Data data = new())
            {
                float? valorTotal = 0;
                int? quantiaTotal = 0;
                List<int?> qtdProdutos = new List<int?>();

                List<ModelProduto> prod = new();
                var Produtos = await data.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
                foreach (var idProduto in Produtos)
                {
                    var ID = idProduto.Object.IdProduto;
                    ModelProduto item = await data.RetornaProdutoPorID(ID) ?? new ModelProduto { };
                    valorTotal += item.PrecoProd * idProduto.Object.QtdPorProd;
                    quantiaTotal += idProduto.Object.QtdPorProd;

                    List<dynamic> unirLista = new();
                    unirLista.Add(item);
                    unirLista.Add(idProduto.Object.QtdPorProd);
                    unirLista.Add(idProduto.Key);
                    ProdutosEqtd.Add(unirLista);

                    // Implementar na model para gerar nota

                    item.Qtd = idProduto.Object.QtdPorProd;
                    item.ValorTotal = (float)Math.Round((decimal)((decimal)item.PrecoProd * idProduto.Object.QtdPorProd), 2);
                    item.Cancelado = false;
                    item.Data = DateTime.Now.ToString("dd/MM/yyyy");
                    prod.Add(item);
                }
                ViewBag.Carrinho = ProdutosEqtd;
                model.ValorTotal = (float)Math.Round((decimal)valorTotal, 2);
                model.Qtd = quantiaTotal;
                if (cardTrocar == null)
                {
                    ViewBag.Cartao = await data.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
                }
                else
                {
                    ViewBag.Cartao = await data.Cartao(HttpContext.Session.GetString("IdUsuario"), cardTrocar);
                }
                ViewBag.ArrayCartoes = await data.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
                if (!string.IsNullOrEmpty(finalizar))
                {
                    var notaFiscal = new ModelNotaFiscal
                    {
                        Cartao = ViewBag.Cartao,
                        Endereço = (await data.RetornaEnderecoPadrao(HttpContext.Session.GetString("IdUsuario"))).Object,
                        Produto = prod
                    };


                    await data.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notaFiscal);
                    for(int i = 0; i < prod.Count; i++)
                    {
                        var produtoRt = await data.RetornaProdutoPorID(prod[i].IdProduto);
                        prod[i].Qtd = produtoRt.Qtd - prod[i].Qtd;
                        prod[i].Data = null;
                        prod[i].Cancelado = null;
                        prod[i].ValorTotal = null;

                        await data.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), prod[i]);
                    }
                    await data.DeletarCarrinho(HttpContext.Session.GetString("IdUsuario"));
                    return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });
                }
            }
            return View(model);
        }
        [HttpGet("Conta/FinalizarCompra/{Excluir}")]
        public async Task<IActionResult> ExcluirCarrinho(string excluir, int quantia)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            if (excluir.Contains("***"))
            {
                var splitExcluir = excluir.Split("***");

                var idprod = splitExcluir[0].Substring(7);
                var quant = splitExcluir[1];
                using (Data data = new())
                {
                    await data.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));
                }
                return RedirectToAction("FinalizarCompra", "Conta");
            }
            using (Data data = new())
            {
                await data.DeleteProdCarrinho(excluir, HttpContext.Session.GetString("IdUsuario"));
            }

            return RedirectToAction("FinalizarCompra", "conta");
        }
        public IActionResult FinalizarPedido(List<string> endereco)
        {
            ModelEndereço model = new();
            model.Endereco = endereco[0];
            model.Numero = Convert.ToInt32(endereco[1]);
            model.Cidade = endereco[2];
            model.UF = endereco[3];
            model.Cep = endereco[4];
            model.Nome = endereco[5];
            return View(model);
        }
        public IActionResult Transacoes()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public IActionResult SeusPedidos()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            return View();
        }
        public IActionResult DetalhePedido()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public IActionResult ExibirRecibo()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public IActionResult VendaNoApp()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VendaNoApp(ModelProduto p)
        {
            var idUser = HttpContext.Session.GetString("IdUsuario") ?? "";
            if (idUser != "") 
            {
                using(ProdutoService prod = new())
                {
                   await prod.SubirImg(_env, p, idUser,0);
                }

                    return RedirectToAction("SuaConta", "Conta");
            }

            return RedirectToAction("Home", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> AlterarProduto(ModelProduto p, string Img,string id)
        {
            p.IdProduto = id;
            if (p.ImgArquivo != null)
            {
               
                if (Img != null)
                {
                    using (Auth auth = new())
                    {
                        using (ProdutoService prod = new())
                        {
                           
                            await prod.SubirImg(_env, p, HttpContext.Session.GetString("IdUsuario"), 1);
                        }
                        await auth.DeleteOneImage(HttpContext.Session.GetString("IdUsuario"), Img);
                    }
                }
            }
            else
            {
                using Data data = new();
                await data.AlterarProduto(HttpContext.Session.GetString("IdUsuario"),p);
            }
            return RedirectToAction("SeusProdutos","Conta");
        }
        public IActionResult AlterarProduto()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public async Task<IActionResult> SeusProdutos()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            using (Data data = new())
            {
                ViewBag.Produtos = await data.RetornarArrayProdutosVendedor(HttpContext.Session.GetString("IdUsuario"));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SeusProdutos(string botao)
        {
            string[] separador = botao.Split("&&");
            string opcao = separador[0];
            string ChaveProd = separador[1];
            string foto = ProdutoService.PegarNomeUrl(separador[2]);
            switch (opcao)
            {
                case "Deletar":
                    using (Data data = new())
                        await data.DeleteUmProduto(ChaveProd);
                    using (Auth auth = new())
                        await auth.DeleteOneImage(HttpContext.Session.GetString("IdUsuario"), foto);
                    return RedirectToAction("SeusProdutos","Conta");
                case "Alterar":
                    return RedirectToAction("AlterarProduto", "Conta", new {Img = foto,Id = separador[3] });
            }
            return View();
        }
        

    }
}