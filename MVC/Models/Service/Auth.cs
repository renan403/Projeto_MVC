
using Firebase.Auth;

namespace MVC.Models.Service

{
    public class Auth : IDisposable
    {
        FirebaseAuthProvider auth;
        private bool disposedValue;

        public Auth()
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyC6L9Knos384ZHZPfVOsaTU5wFldlB1JMs"));
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
                await auth.SignInWithEmailAndPasswordAsync(email, pwd);
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

        public async Task<string> DeleteEmail(string email, string pwd)
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
