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
        public async Task Teste()
        {
            //var test = await VerificaPadrao("renancporto94@gmail.com");
            
            
            
        }

        public async Task<bool> IsUserExists(string email)
        {
            var user = (await client.Child("Usuarios")
                .OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .FirstOrDefault();
            return (user != null);
        }
        public async Task<bool> RegisterUser(string? name, string? email, string? senha)
        {
            if (name == null || email == null || senha.Length < 8 )
            {
                return false;
            }
            if (await IsUserExists(email) == false)
            {
                await client.Child("Usuarios")
                    .PostAsync(new ModelUsuario()
                    {
                        Nome = name,
                        Senha = senha,
                        Email = email                        
                    });
                return true;
            }
            else
            {
                return false;
            }
        }
        //public async Task<bool> ValidaCod(string? cod, string? email)
        //{
        //    if (cod == null || email == null)
        //    {
        //        return false;
        //    }
        //    var user = (await client.Child("Usuarios/").OnceAsync<ModelUsuario>())
        //        .Where(u => u.Object.Codigo == cod)
        //        .Where(u => u.Object.Email == email)
        //        .FirstOrDefault();
        //    if (user != null)
        //    {
        //        await AtivarUser(email);
        //    }

        //    return (user != null);
        //}
        //private async Task<bool> AtivarUser(string? email)
        //{

        //    var userSenha = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Senha;
        //    var userNome = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
        //    var chave = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;

        //    await client.Child($"Usuarios/{chave}")
        //            .PatchAsync(new Models.ModelUsuario()
        //            {
        //                Nome = userNome,
        //                Senha = userSenha,
        //                Email = email,                     
        //                Ativo = true
        //            });
        //    return true;

        //}
        //public async Task<bool> ReenviarCodigo(string? email, string? codigo)
        //{
        //    if (email == null || codigo == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        var userSenha = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Senha;
        //        var userNome = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
        //        var chave = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;

        //        await client.Child($"Usuarios/{chave}")
        //                .PatchAsync(new Models.ModelUsuario()
        //                {
        //                    Nome = userNome,
        //                    Senha = userSenha,
        //                    Email = email,
        //                    Codigo = codigo,
        //                });
        //        return true;
        //    }
        //}
        public async Task<bool> LoginUser(string? email, string? senha)
        {
            if (email != null || senha != null)
            {

                var user = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>())
                    .Where(u => u.Object.Email == email)
                    .Where(u => u.Object.Senha == senha)
                    .Where(u => u.Object.Ativo == true)
                    .FirstOrDefault();
                return (user != null);
            }
            return false;
        }
        public async Task<string> RetornaNome(string? email, string? senha)
        {

            var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .Where(u => u.Object.Senha == senha)
                .FirstOrDefault();
            if (user != null)
            {
                var nome = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
                return nome ?? "";
            }


            return string.Empty;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<FirebaseObject<ModelEndereço>?> RetornaEnderecoPadrao(string? email)
        {

            var user = (await client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)             
                .FirstOrDefault();
            if (user != null)
            {
                var enderecos = (await PuxarEndereco(email)).ToArray();
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
        public async Task<bool> SalvarEndereco(string email, string pais, string nome, string telefone, string cep, string endereco, int numResid, string complemento, string bairro, string cidade, string estado, bool padrao, string uf)
        {
            if (email == "" || pais == "" || nome == "" || telefone == "" || cep == "" || endereco == "" || numResid == 0 || bairro == "" || cidade == "" || estado == "" || uf == "")
            {
                return false;
            }
            var chave = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;
            var count = (await client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelUsuario>());
            var EnderecoConvert = count.ToArray();

            for (int i = 0; i <= count.Count; i++)
            {
                try
                {

                    if (!EnderecoConvert[i].Key.Contains($"End{i + 1}"))
                    {
                        var possuiPadrao = await VerificaPadrao(email);
                        if (possuiPadrao)
                        {
                            await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(new ModelEndereço()
                            {
                                Nome = nome,
                                Telefone = telefone,
                                Cep = cep,
                                Endereco = endereco,
                                Pais = pais,
                                Numero = numResid,
                                Complemento = complemento,
                                Bairro = bairro,
                                Cidade = cidade,
                                Estado = estado,
                                Padrao = padrao,
                                UF = uf
                            });
                        }
                        else
                        {
                            await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(new ModelEndereço()
                            {
                                Nome = nome,
                                Telefone = telefone,
                                Cep = cep,
                                Endereco = endereco,
                                Pais = pais,
                                Numero = numResid,
                                Complemento = complemento,
                                Bairro = bairro,
                                Cidade = cidade,
                                Estado = estado,
                                Padrao = true,
                                UF = uf
                            });
                        }
                        break;
                    }

                }
                catch
                {
                    var possuiPadrao = await VerificaPadrao(email);
                    if (possuiPadrao)
                    {
                        await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(new ModelEndereço()
                        {
                            Nome = nome,
                            Telefone = telefone,
                            Cep = cep,
                            Endereco = endereco,
                            Pais = pais,
                            Numero = numResid,
                            Complemento = complemento,
                            Bairro = bairro,
                            Cidade = cidade,
                            Estado = estado,
                            Padrao = padrao,
                            UF = uf
                        });
                    }
                    else
                    {
                        await client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(new ModelEndereço()
                        {
                            Nome = nome,
                            Telefone = telefone,
                            Cep = cep,
                            Endereco = endereco,
                            Pais = pais,
                            Numero = numResid,
                            Complemento = complemento,
                            Bairro = bairro,
                            Cidade = cidade,
                            Estado = estado,
                            Padrao = true,
                            UF = uf
                        });
                    }


                }

            }
            return true;
        }
        public async Task<bool> AlterarEndereco(string? key, string email, string? pais, string? nome, string? telefone, string? cep, string? endereco, int numResid, string? complemento, string? bairro, string? cidade, string? estado, bool padrao, string? uf)
        {
            if (key == null || email == String.Empty)
                return false;
            var chave = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;
            var puxarEnd = await PuxarEndereco(email);
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
        public async Task<bool> DeleteEndereco(string Key, string? email)
        {
            if (Key.Contains("End") && email != null)
            {
                var chave = (await client.Child("Usuarios").OnceAsync<ModelLogin>()).Where(m => m.Object.Email == email).FirstOrDefault().Key;
                await client.Child($"Usuarios/{chave}/Endereco/{Key}").DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IReadOnlyCollection<FirebaseObject<ModelEndereço>>?> PuxarEndereco(string email)
        {
            if (email != String.Empty)
            {
                var chave = (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;
                var enderecoResult = (await client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelEndereço>());

                return enderecoResult;
            }
            else
            {
                return null;
            }


        }
        public async Task<bool> VerificaPadrao(string email)
        {
            if (email != null)
            {
                var enderecos = (await PuxarEndereco(email)).ToArray();
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
        public async Task<bool> DeleteUser()
        {
            await client.Child("Usuarios").DeleteAsync();

            return true;
        }
      
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

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


