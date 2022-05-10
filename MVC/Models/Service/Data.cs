using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
        public async Task<bool> IsUserExists(string email)
        {
            var user = (await client.Child("Usuarios")
                .OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .FirstOrDefault();
            return (user != null);
        }
        public async Task<bool> RegisterUser(string? name, string? email, string? senha,string? codigo)
        {
            if(name == null || email == null || senha.Length < 8 || codigo == null)
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
                        Email = email,
                        Codigo = codigo,
                        Ativo = false
                    });
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> ValidaCod(string? cod, string? email)
        {
            if (cod == null || email == null)
            {
                return false;
            }
            var user = (await client.Child("Usuarios/").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Codigo == cod)
                .Where(u => u.Object.Email == email)
                .FirstOrDefault();
            if (user != null)
            {
                await AtivarUser(email);
            }

            return (user != null);
        }
        private async Task<bool> AtivarUser(string? email)
        {

            var userSenha = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Senha;
            var userNome = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
            var codigo = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Codigo;
            var chave = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;

            await client.Child($"Usuarios/{chave}")
                    .PatchAsync(new Models.ModelUsuario()
                    {
                        Nome = userNome,
                        Senha = userSenha,
                        Email = email,
                        Codigo = codigo,
                        Ativo = true
                    });
            return true;

        }
        public async Task<bool> ReenviarCodigo(string? email, string? codigo)
        {
            if (email == null || codigo == null)
            {
                return false;
            }
            else
            {
                var userSenha = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Senha;
                var userNome = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
                var chave = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;

                await client.Child($"Usuarios/{chave}")
                        .PatchAsync(new Models.ModelUsuario()
                        {
                            Nome = userNome,
                            Senha = userSenha,
                            Email = email,
                            Codigo = codigo,
                        });
                return true;
            }
        }
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

            var user = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .Where(u => u.Object.Senha == senha)
                .FirstOrDefault();
            if(user != null)
            {
               var nome = (await client.Child("Usuarios").OnceAsync<Models.ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Nome;
                return nome ?? "";
            }


            return string.Empty;
        }

        //public async Task<bool> ValidaUser(string email, String cod)
        //{
        //    var userSenha = (await client.Child("Usuarios").OnceAsync<User>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Password;
        //    var userName = (await client.Child("Usuarios").OnceAsync<User>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Username;
        //    var chave = (await client.Child("Usuarios").OnceAsync<User>()).Where(u => u.Object.Email == email).FirstOrDefault().Key;

        //    await client.Child($"Usuarios/{chave}")
        //            .PatchAsync(new User()
        //            {
        //                Username = userName,
        //                Password = userSenha,
        //                Email = email,
        //                Codigo = cod,
        //                Ativo = true,
        //            });
        //    return true;
        //}
        public async Task<bool> DeleteUser()
        {
            await client.Child("Usuarios").DeleteAsync();

            return true;
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Data()
        // {
        //     // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


}


