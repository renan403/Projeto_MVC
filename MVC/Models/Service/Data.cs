using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Firebase.Auth;

//Fazer DLL do Banco
//Fazer encapsulamento

namespace MVC.Models.Service
{
    public class Data : IDisposable
    {
        FirebaseClient client;

        private bool disposedValue;

        public Data()
        {
            client = new FirebaseClient("https://projetoport-50b66-default-rtdb.firebaseio.com/");

        }
        private async Task<string> GetUserKey(string userId)
        {
            return (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(m => m.Object.IdUser == userId).FirstOrDefault().Key;
        }
        public async Task Teste()
        {
            var card = await RetornarNotaFiscal("NVjR6VeORB2hOTeZBdOcbTLq6MC5dkfD8tETNuHd");
           // var test = await PegarItemUnico("-N6Aj_d2ZbUlbF-2iXeh");
        
        }
        public async Task<Array> RetornarNotaFiscal(string userId)
        {
            var key = await GetUserKey(userId);
            var notas = await client.Child($"Usuarios/{key}/NotaFiscal").OnceAsync<ModelNotaFiscal>();
            return notas.ToArray();
        }
        public async Task<bool> GerarNotaFiscal(string userId,ModelNotaFiscal notaFiscal)
        {
            Gerador gera = new();
            for (int i = 0; i < notaFiscal.Produto.Count; i++)
            {
                notaFiscal.Produto[i].Data = DateTime.Now.ToString("dd/MM/yyyy");
                notaFiscal.Produto[i].Enviado = true;
                notaFiscal.Produto[i].Recebido = false;
            }
            
            string numPedido = $"{ gera.aleatoriosComprarProd() }-{ gera.aleatoriosComprarProd()}";
            var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/NotaFiscal/Pedido-{numPedido}").PatchAsync(notaFiscal);
            return true;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------
        //               Compra
        public async Task AddItemUnico(string userId, ModelProduto produto)
        {
            var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/Produto/").PatchAsync(produto);

            var prod = RetornaProdutoPorID(produto.IdProduto);

        }
        private async Task<ModelProduto> PegarItemUnico(string key)
        {
           return (await client.Child($"Usuarios/{key}/Produto/").OnceSingleAsync<ModelProduto>());
        }
        public async Task AlterarItemUnico(string userId, int qtd)
        {
            var key = await GetUserKey(userId);
            var item = await PegarItemUnico(key);
            item.ValorTotal = (float)Math.Round(((decimal)item.PrecoProd * qtd ), 2); ;
            item.Qtd = qtd;
            await AddItemUnico(userId,item);
        }
        public async Task<ModelProduto>PegarProduto(string userId)
        {
            var key = await GetUserKey(userId);
            ModelProduto model = (await client.Child($"Usuarios/{key}/Produto/").OnceSingleAsync<ModelProduto>());
            return model;
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //               Cartão
        public async Task<string> SalvarCard(string userId,ModelCartao model)
        {
            string cvvstring =model.Cvv.ToString();
            if (model.Bandeira == "Amex" && cvvstring.Length < 3)
            {
                return "⚠️ Cartão não adicionado CVV Faltando 1 número.";
            }
            else if (model.Bandeira != "Amex" && cvvstring.Length > 3)
            {
                return "⚠️ Cartão não adicionado CVV está com 1 número a mais.";
            }
            else
            {
                model.Padrao = false;
                model.QuatroDigCard = model.NumeroCard.Substring(model.NumeroCard.Length - 4);
                var key = await GetUserKey(userId);
                var count = await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelProduto>();
                var cartoes = count.ToArray();
                for (int i = 0; i <= cartoes.Length; i++)
                {
                    try
                    {
                        if (!cartoes[i].Key.Contains($"Cartao{i + 1}"))
                        {
                            await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }

                    }
                    catch
                    {
                        if(await VerificaCardPadrao(userId))
                        {
                            await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }
                        else
                        {
                            model.Padrao = false;
                            await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }
                       
                    }

                }          
            }
            return "falha";
        }
        public async Task<IReadOnlyCollection<FirebaseObject<ModelCartao>>> ReturnCard(string userId)
        {
            var key = await GetUserKey(userId);
            return (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).ToArray(); 
            
        }
        public async Task DeleteCard(string userId, string cartao)
        {
          var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/Card/{cartao}").DeleteAsync();
        }
        public async Task<bool> AlterarCard(string userId, string cartao, string nome, string data)
        {
            var key = await GetUserKey(userId);
            var dadosCard = (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).Where(m => m.Key == cartao).FirstOrDefault();
            if(dadosCard == null)
                return false;
            await client.Child($"Usuarios/{key}/Card/{cartao}").PatchAsync(new ModelCartao()
            {
                NomeCard = nome,
                Cvv = dadosCard.Object.Cvv,
                CaminhoImgBandeira = dadosCard.Object.CaminhoImgBandeira,
                NumeroCard = dadosCard.Object.NumeroCard,
                Bandeira = dadosCard.Object.Bandeira,
                DataExpiracao = data,
                Erro = dadosCard.Object.Erro,
                QuatroDigCard = dadosCard.Object.QuatroDigCard
            }) ;
            return true;
        }
        public async Task<ModelCartao> Cartao(string userId, string keyCard)
        {
            var key = await GetUserKey(userId);
            return (await client.Child($"Usuarios/{key}/Card/{keyCard}").OnceSingleAsync<ModelCartao>());            
        }
        public async Task<bool> VerificaCardPadrao(string userId)
        {
            if (userId != null)
            {
                var cartoes = (await ReturnCard(userId)).ToArray();
                foreach (var cartao in cartoes)
                {
                    if (cartao.Object.Padrao)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        public async Task<FirebaseObject<ModelCartao>?> RetornarCartaoPadrao(string userId)
        {
            var key = await GetUserKey(userId);

            var cartoes = (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>());
            foreach(var card in cartoes)
            {
                if (card.Object.Padrao)
                    return card;
            }
           
            return null;
        }
        public async Task<bool> MudarPadraoCard(string userId,string key)
        {
            var chave = await GetUserKey(userId);
            var cartao = await Cartao(userId, key);
            try
            {
                var CardAtualPadrao = await RetornarCartaoPadrao(userId);
                CardAtualPadrao.Object.Padrao = false;
                await client.Child($"Usuarios/{chave}/Card/{CardAtualPadrao.Key}").PatchAsync(CardAtualPadrao.Object);
                cartao.Padrao = true;
                await client.Child($"Usuarios/{chave}/Card/{key}").PatchAsync(cartao);
                return true;
            }
            catch
            {
                return false;
            }
           
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //              Informaçoes do User
        public async Task<bool> IsUserExists(string email)
        {
            var user = (await client.Child("Usuarios")
                .OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .FirstOrDefault();
            return (user != null);
        }
        public async Task<bool> RegisterUser(string? name, string? email)
        {
            string id = string.Empty;
            using Gerador gerador = new();
            id = gerador.aleatoriosUser();
            if (name == null || email == null)
            {
                return false;
            }
            if (await IsUserExists(email) == false)
            {
                await client.Child("Usuarios")
                    .PostAsync(new ModelUsuario()
                    {
                        IdUser = id,
                        Nome = name,
                        Email = email
                    });
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> LoginUser(string? email)
        {
            if (email != null)
            {

                var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>())
                    .Where(u => u.Object.Email == email)
                    .FirstOrDefault();
                return (user != null);
            }
            return false;
        }
        public async Task<string> RetornaNome(string? idUser)
        {

            var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.IdUser == idUser)
                .FirstOrDefault();
            if (user != null)
            {
                var nome = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.IdUser == idUser).FirstOrDefault().Object.Nome;
                return nome ?? "";
            }


            return string.Empty;
        }
        public async Task<string> RetornaID(string? email)
        {
            try
            {
                var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Email;
                if (user != null)
                {
                    var id = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.IdUser;
                    return id ?? "";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }
        

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //              Endereço
        public async Task<FirebaseObject<ModelEndereço>?> RetornaEnderecoPadrao(string? idUser)
        {

            var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.IdUser == idUser)
                .FirstOrDefault();
            if (user != null)
            {
                var enderecos = (await PuxarEndereco(idUser)).ToArray();
                foreach (var item in enderecos)
                {
                    if (item.Object.Padrao)
                    {
                        return item;
                    }

                }
            }
            return null;
        }
        public async Task<bool> SalvarEndereco(string userId,ModelEndereço model)
        {
            if (userId == "" || model.Pais == "" || model.Nome == "" || model.Telefone == "" || model.Cep == "" || model.Endereco == "" || model.Numero == 0 || model.Bairro == "" || model.Cidade == "" || model.Estado == "" || model.UF == "")
            {
                return false;
            }
            var chave = await GetUserKey(userId); 
            var count = (await client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelUsuario>());
            var EnderecoConvert = count.ToArray();

            for (int i = 0; i <= count.Count; i++)
            {
                try
                {

                    if (!EnderecoConvert[i].Key.Contains($"End{i + 1}"))
                    {
                        var possuiPadrao = await VerificaPadrao(userId);
                        if (possuiPadrao)
                        {

                            await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                            if (model.Padrao == true)
                            {
                                await MudarPadrao(userId, $"End{i + 1}");
                            }
                        }
                        else
                        {
                            model.Padrao = true;
                            await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                        }
                        break;
                    }

                }
                catch
                {
                    var possuiPadrao = await VerificaPadrao(userId);
                    if (possuiPadrao)
                    {
                        await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                        if (model.Padrao == true)
                        {
                            await MudarPadrao(userId,$"End{i + 1}");
                        }
                        
                    }
                    else
                    {
                        model.Padrao = true;
                        await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                    }


                }

            }
            return true;
        }
        public async Task<bool> AlterarEndereco(string? key, string userId, string? pais, string? nome, string? telefone, string? cep, string? endereco, int numResid, string? complemento, string? bairro, string? cidade, string? estado, bool padrao, string? uf)
        {
            if (key == null || userId == String.Empty)
                return false;
            var chave = await GetUserKey(userId);
            var puxarEnd = await PuxarEndereco(userId);
            var end = puxarEnd?.Where(m => m.Key == key).FirstOrDefault();
            if (end == null)
                return false;
            await client.Child($"Usuarios/{chave}/Endereco/{end.Key}").PatchAsync(new ModelEndereço()
            {
                Nome = nome ?? end.Object.Nome,
                Telefone = telefone ?? end.Object.Telefone,
                Cep = cep ?? end.Object.Cep,
                Endereco = endereco ?? end.Object.Endereco,
                Pais = pais ?? end.Object.Pais,
                Numero = numResid == 0 ? end.Object.Numero : numResid,
                Complemento = complemento ?? end.Object.Complemento,
                Bairro = bairro ?? end.Object.Bairro,
                Cidade = cidade ?? end.Object.Cidade,
                Estado = estado ?? end.Object.Estado,
                Padrao = end.Object.Padrao,
                UF = uf ?? end.Object.UF
            });
            return true;
        }
        public async Task<bool> DeleteEndereco(string Key, string? userId)
        {
            if (Key.Contains("End") && userId != null)
            {
                var chave = await GetUserKey(userId);
                await client.Child($"Usuarios/{chave}/Endereco/{Key}").DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IReadOnlyCollection<FirebaseObject<ModelEndereço>>?> PuxarEndereco(string userId)
        {
            if (userId != String.Empty)
            {
                var chave = await GetUserKey(userId);
                var enderecoResult = (await client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelEndereço>());

                return enderecoResult;
            }
            else
            {
                return null;
            }


        }
        public async Task<bool> VerificaPadrao(string idUser)
        {
            if (idUser != null)
            {
                var enderecos = (await PuxarEndereco(idUser)).ToArray();
                foreach (var item in enderecos)
                {
                    if (item.Object.Padrao)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        public async Task<bool> DeleteUser(string email)
        {
            var key = (await client.Child("Usuarios").OnceAsync<ModelLogin>()).Where(m => m.Object.Email == email).FirstOrDefault().Key;
            await client.Child($"Usuarios/{key}").DeleteAsync();

            return true;
        }
        public async Task<bool> MudarPadrao(string userId, string key)
        {
            if (key == null || userId == String.Empty)
                return false;
            var chave = await GetUserKey(userId);
            var puxarEnd = await PuxarEndereco(userId);
            var end = puxarEnd?.Where(m => m.Key == key).FirstOrDefault();
            if (end == null)
                return false;

            var enderecos = ((await PuxarEndereco(userId)).Where(m => m.Object.Padrao == true)).FirstOrDefault();

            enderecos.Object.Padrao = false;
            await client.Child($"Usuarios/{chave}/Endereco/{enderecos.Key}").PatchAsync(enderecos.Object);

            end.Object.Padrao = true;
            await client.Child($"Usuarios/{chave}/Endereco/{end.Key}").PatchAsync(end.Object);


            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                  Produtos

        public async Task<bool> AddProduto(string? idUser,ModelProduto model)
        {
            
            if (idUser != null || idUser == "")
            {
                using Gerador gerador = new();
                var idProd = gerador.aleatoriosProd();
                model.IdProduto = idProd;
                model.IdUser = idUser;
                await client.Child($"Produtos").PostAsync(model) ;

                return true;
            }
            return false;
        }
        public async Task<Array> RetornaArrayProdutos()
        {
            return (await client.Child("Produtos").OnceAsync<ModelProduto>()).ToArray();
        }
        public async Task<Array> RetornarArrayProdutosVendedor(string idUser)
        {
            return (await client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdUser == idUser).ToArray();           
        }
        public async Task<bool> DeleteProdutos(string userId)
        {
            var key = await GetUserKey(userId);
            await client.Child($"Produtos/{key}").DeleteAsync();
            return true;
        }
        public async Task DeleteUmProduto(string keyProd)
        {        
            await client.Child($"Produtos/{keyProd}").DeleteAsync();
        }
        public async Task<ModelProduto?> RetornaProdutoPorID(string idProd)
        {
            if (idProd != "")
            {
                var retorno = (await client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdProduto == idProd).FirstOrDefault().Object;
                return retorno;
            }
            return null;
        }
        public async Task<bool> AlterarProduto(string? userId, ModelProduto model)
        {
            var key = await GetUserKey(userId);
            var objt =await RetornaProdutoPorID(model.IdProduto);
            var retorno = (await client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdProduto == model.IdProduto).FirstOrDefault().Key;
            objt.ImgArquivo = model.ImgArquivo ?? objt.ImgArquivo;
            objt.Categoria = model.Categoria ?? objt.Categoria;
            objt.Data = model.Data ?? objt.Data;
            objt.DescriProd = model.DescriProd ?? objt.DescriProd;
            objt.ValorTotal = model.ValorTotal ?? objt.ValorTotal;
            objt.Cancelado = model.Cancelado ?? objt.Cancelado;
            objt.MarcaProd = model.MarcaProd ?? objt.MarcaProd;
            objt.PrecoProd = model.PrecoProd ?? objt.PrecoProd;
            objt.Qtd = model.Qtd ?? objt.Qtd;
            objt.UrlImg = model.UrlImg ?? objt.UrlImg;
            objt.NomeProd = model.NomeProd ?? objt.NomeProd;
            objt.ModeloProd = model.ModeloProd ?? objt.ModeloProd;

            await client.Child($"Produtos/{retorno}").PatchAsync(objt);
            return false;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                  Carrinho

        public async Task<IReadOnlyCollection<FirebaseObject<ModelProduto>>> RetornaCarrinho(string userId)
        {
            var KeyUser = await GetUserKey(userId);
            return (await client.Child($"Usuarios/{KeyUser}/Carrinho").OnceAsync<ModelProduto>());

        }
        public async Task<bool> SalvarProdCarrinho(string userId, string idprod , int quantidade)
        {
            var KeyUser = await GetUserKey(userId);
            var count = await client.Child($"Usuarios/{KeyUser}/Carrinho").OnceAsync<ModelProduto>();
            var carrinho = count.ToArray();

            for (int i = 0; i <= carrinho.Length; i++)
            {
                try
                {
                    if (!carrinho[i].Key.Contains($"Produto{i + 1}"))
                    {
                        await client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod , QtdPorProd = quantidade});
                        return true;
                    }

                }
                catch
                {
                    await client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod, QtdPorProd = quantidade });
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> DeleteProdCarrinho(string Key, string userId)
        {
            if (Key.Contains("Produto") && userId != null)
            {
                var chave = await GetUserKey(userId);
                await client.Child($"Usuarios/{chave}/Carrinho/{Key}").DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public async Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade)
        {
            var carrinho = await RetornaCarrinho(userId);

            foreach(var car in carrinho)
            {
                if(car.Object.IdProduto == idProd)
                {
                    var key = await GetUserKey(userId);
                    await client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Object.IdProduto,
                        QtdPorProd = quantidade
                    });
                    return true;
                }
            }
            return false;
        }
        public async Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade)
        {
            var carrinho = await RetornaCarrinho(userId);

            foreach (var car in carrinho)
            {
                if (car.Object.IdProduto == idProd)
                {
                    if(car.Object.QtdPorProd + quantidade > 5)
                    {
                        return $"{car.Object.QtdPorProd}";
                    }
                    var key = await GetUserKey(userId);
                    await client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Object.IdProduto,
                        QtdPorProd = car.Object.QtdPorProd + quantidade
                    });
                    return "sucesso";
                }
            }
            return "erro";
        }
        public async Task<bool> PossuiNoCarrinho(string idClient, string idProd)
        {
            var carrinho = await RetornaCarrinho(idClient);

            foreach (var car in carrinho)
            {
                if (car.Object.IdProduto == idProd)
                {
                    return true;
                }
            }
            return false;
        }        
        public async Task DeletarCarrinho(string userId)
        {
            var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/Carrinho").DeleteAsync();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



    }
}


