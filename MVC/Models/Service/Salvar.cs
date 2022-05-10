using System.Security.Cryptography;
using System.Text;
using Seguranca_v2;

namespace MVC.Models.Service
{
    public class Salvar:Chave
    {
        public byte[] Criptografar(string txt)
        {
            using (Aes myAes = Aes.Create())
            {
                byte[] chaveByte = Encoding.UTF8.GetBytes(chave);
                byte[] encriptado = Encriptar.EncryptarStringParaByte(txt, chaveByte, myAes.IV);
                return encriptado;
            }
            
        }
        public string Decriptografar(string txtEncriptado)
        {
            byte[] txtEncrypt = Encoding.UTF8.GetBytes(txtEncriptado);
            using (Aes myAes = Aes.Create()) {
                byte[] chaveByte = Encoding.UTF8.GetBytes(chave);
                string Decriptado = Decriptar.DecryptarStringDeBytes(txtEncrypt, chaveByte, myAes.IV);
                return Decriptado;
            }
           
        }
    }
}