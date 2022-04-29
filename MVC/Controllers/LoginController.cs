using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using MVC.Control.InfoDAO;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {   
        ModelController model = new ModelController();
        Data data = new Data();
        Salvar salvar = new Salvar();
        public async Task<ActionResult> Login()
        {
           
            return View();
        }
        public async Task<IActionResult> CriarConta(string nome, string email, string senha,string confSenha)
        {
            try
            {
                if (senha != null)
                {
                    if (senha == confSenha)
                    {
                        using (Salvar salvar = new Salvar())
                        {
                           bool resultado = await data.RegisterUser(nome, email, salvar.Criptografar(senha));
                            if (resultado == false)
                            {
                                model.Error = "Cadastrado";
                            }
                        }
                        
                    }
                    else
                    {
                        model.Error = "Divergente";
                    }
                   
                }
                else
                {
                    model.Error = "Vazio";
                }
               

            }
            catch (Exception)
            {

                throw;
            }
            
            return View(model);
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
