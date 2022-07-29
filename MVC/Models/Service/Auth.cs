
using Firebase.Auth;
using Firebase.Storage;
using System.IO;

namespace MVC.Models.Service

{
    public class Auth : IDisposable
    {
        readonly FirebaseAuthProvider auth;
        private bool disposedValue;
        public Auth()
        {                 
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyC6L9Knos384ZHZPfVOsaTU5wFldlB1JMs"));
        }   
        public async Task<string> UploadImage(IWebHostEnvironment env, IFormFile? file, string userID,string email,string senha)
        {
            if(file.Length < 1)
            {
                return String.Empty;                                                                      
            }                                                                                             
            string? downloadUrl = null;                                                                   
            var token = await auth.SignInWithEmailAndPasswordAsync(email, senha);
            var path = Path.Combine(env.WebRootPath, $"img\\Temp\\{file.FileName}");                      
            var canc = new CancellationTokenSource();                                                     
            using (FileStream fs = new(path, FileMode.Open))
            {
                var storage = new FirebaseStorage("projetoport-50b66.appspot.com",
                          new FirebaseStorageOptions
                          {
                              AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                              ThrowOnCancel = true
                          }).Child("products")
                            .Child(userID)
                            .Child(file.FileName)
                            .PutAsync(fs, canc.Token);
              downloadUrl = await storage;
            }

            
            File.Delete(path);
            try
            {

                return $"{downloadUrl}" ;
            }
            catch (Exception ex)
            {
                return $"Exception was thrown: {ex}";
            }
        }
        public async Task  DeleteImage(Array produtos, string IdUser, string email, string senha)
        {
            try
            {
                foreach (var NomeDaFoto in produtos)
                {
                    var token = await auth.SignInWithEmailAndPasswordAsync(email, senha);
                    var storage = new FirebaseStorage("projetoport-50b66.appspot.com",
                              new FirebaseStorageOptions
                              {
                                  AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                  ThrowOnCancel = true
                              }).Child("products").Child(IdUser).Child(NomeDaFoto.ToString()).DeleteAsync();

                    storage.Wait();
                }
               
            }
            catch (Exception ex)
            {

                throw;
            }
            

        }
        public async Task DeleteOneImage(string userId,string email, string senha ,string nomeImg )
        {
            try
            {
                var token = await auth.SignInWithEmailAndPasswordAsync(email, senha);
                TimeSpan time = new TimeSpan(0, 0,15);
                var canc = new CancellationTokenSource();
                var storage = new FirebaseStorage("projetoport-50b66.appspot.com",
                          new FirebaseStorageOptions
                          {
                              AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                              ThrowOnCancel = true
                          }).Child("products").Child(userId).Child(nomeImg).DeleteAsync();
                storage.Wait();
            }
            catch (Exception ex)
            {
             
                throw;
            }


        }
        public async Task<string> RegisterEmail(string? email, string? pwd, string? name)
        {
            if (email == "" || pwd == "" || name == "")
                return "Empty";

            try
            {
                await auth.CreateUserWithEmailAndPasswordAsync(email, pwd, name, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    return "Email ja Existe";
                }
                return  "Outro erro";
            }
            return "Criado";
        }
        public async Task<string> LoginEmail(string email, string pwd)
        {
            if (email == "" || pwd == "")
                return "Empty";
            try
            {
               var result =  await auth.SignInWithEmailAndPasswordAsync(email, pwd);
                if (!result.User.IsEmailVerified)
                    return "UNAUTHENTICATED";
                
            }catch(Exception ex)
            {

                if (ex.Message.Contains("INVALID_PASSWORD"))
                    return "INVALID_PASSWORD";

                if (ex.Message.Contains("EMAIL_NOT_FOUND"))
                    return "EMAIL_NOT_FOUND";
                else
                    return "Other error";
            }
           

            return "Logged";
        }
        public async Task<string> ChangeEmail(string email, string pwd, string newEmail)
        {
            FirebaseAuthLink idToken;
            try
            {
                 idToken = await auth.SignInWithEmailAndPasswordAsync(email, pwd);
                 await auth.ChangeUserEmail(idToken.FirebaseToken, newEmail);
                 return "Altered";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INVALID_PASSWORD"))
                    return "INVALID_PASSWORD";

                if (ex.Message.Contains("EMAIL_NOT_FOUND"))
                    return "EMAIL_NOT_FOUND";
                else
                    return "Other error";
            }                       
        }
        public async Task<string> ChangePassword(string email, string pwd , string newPwd)
        {

            FirebaseAuthLink idToken;
            try
            {
               
                idToken = await auth.SignInWithEmailAndPasswordAsync(email, pwd);
                await auth.ChangeUserPassword(idToken.FirebaseToken, newPwd);
                return "Altered";
            }catch(Exception ex)
            {
                if (ex.Message.Contains("INVALID_PASSWORD"))
                    return "WEAK_PASSWORD";
                if (ex.Message.Contains("INVALID_PASSWORD"))
                    return "INVALID_PASSWORD";
                return "Other erro";
            }
            
        }
        public async Task<string> ResetPassword(string email)
        {
            try
            {
                await auth.SendPasswordResetEmailAsync(email);
                return "Altered";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_NOT_FOUND"))
                    return "EMAIL_NOT_FOUND";
                if (ex.Message.Contains("MISSING_EMAIL"))
                    return "MISSING_EMAIL";
                return "Other erro";
            }
        }
        public async Task<string> DeleteUser(string email, string pwd)
        {          
            FirebaseAuthLink idToken;
            try
            {
                idToken = await auth.SignInWithEmailAndPasswordAsync(email, pwd);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INVALID_PASSWORD"))
                    return "INVALID_PASSWORD";

                if (ex.Message.Contains("EMAIL_NOT_FOUND"))
                    return "EMAIL_NOT_FOUND";
                else
                    return "Other error";
            }
           

            using (Data data = new())
            {
                var id = await data.RetornaID(email);
                await data.DeleteUser(email);
            }
            await auth.DeleteUserAsync(idToken.FirebaseToken);

            return "Deleted";
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
        // ~Login()
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
