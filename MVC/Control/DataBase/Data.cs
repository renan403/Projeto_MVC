using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace MVC.Control.DataBase
{
    public class Data:IDisposable
    {
        
            /*public string email { get; set; }
            public string nome { get; set; }
            public string senha { get; set; }
            public string codigo { get; set; }
            */
            FirebaseClient client;
        private bool disposedValue;

        public Data()
            {
                client = new FirebaseClient("https://projetoport-50b66-default-rtdb.firebaseio.com/");
            }
            public async Task<bool> IsUserExists(string email)
            {
                var user = (await client.Child("Usuarios")
                    .OnceAsync<Models.ModelController>())
                    .Where(u => u.Object.Email == email)
                    .FirstOrDefault();
                return (user != null);
            }
            public async Task<bool> RegisterUser(string name,string email, byte[] senha)
            {
                if (await IsUserExists(name) == false)
                {
                    await client.Child("Usuarios")
                        .PostAsync(new Models.ModelController()
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
        
            //public async Task<bool> LoginUser(string email, string senha, bool ativo)
            //{
            //    var user = (await client.Child("Usuarios").OnceAsync<User>())
            //        .Where(u => u.Object.Email == email)
            //        .Where(u => u.Object.Password == senha)
            //        .Where(u => u.Object.Ativo == ativo)
            //        .FirstOrDefault();
            //    return (user != null);
            //}
            //public async Task<bool> ValidaCod(String cod, String email)
            //{
            //    var user = (await client.Child("Usuarios/").OnceAsync<User>())
            //        .Where(u => u.Object.Codigo == cod)
            //        .Where(u => u.Object.Email == email)
            //        .FirstOrDefault();
            //    return (user != null);
            //}
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


