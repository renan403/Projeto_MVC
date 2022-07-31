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
            //return RedirectToAction("DetalhePedido", "Conta");
            using Auth auth = new();
         

            using (Data data = new())
            {

               //var quant = (await data.RetornarArrayProdutosVendedor("aK8aShCLmIjmRRerc1ErIl7THjjmPTOG1GH7cPFT")).Length;
                await data.Teste();
                ViewBag.Produtos = await data.RetornaArrayProdutos();

            }
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var sessaoEmail = HttpContext.Session.GetString("SessaoEmail");
            return View();
        }
        [HttpGet("/Home/Home/Sair")]
        public IActionResult HomeSair()
        {
            HttpContext.Session.SetString("SessaoNome", "");
            HttpContext.Session.SetString("Endereço", "");
            HttpContext.Session.SetString("nomeFormatado", "");
            HttpContext.Session.SetString("IdUsuario", "");
            HttpContext.Session.SetString("Senha", "");
            return RedirectToAction("Home", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}