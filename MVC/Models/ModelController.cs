namespace MVC.Models
{
    public class ModelController
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cep { get; set; }
        public bool Login { get; set; }
        public byte[]? Senha { get; set; }
        public string? Error { get; set; }
    }
}
