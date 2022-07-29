namespace MVC.Models
{
    public class ModelGeral
    {
        ModelCartao? Cartao { get; set; }
       public string? Email { get; set; }
        ModelEndereco? Endereco { get; set; }
        public string? idUser { get; set; }
        public string? Nome { get; set; }
        ModelNotaFiscal? notaFiscal { get; set;}
    }
}
