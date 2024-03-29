﻿namespace MVC.Models
{
    public class ModelProduto
    {
        public string? NomeProd { get; set; }
        public string? NomeVendedor { get; set; }
        public string? MarcaProd { get; set; }
        public string? ModeloProd { get; set; }
        public string? DescriProd { get; set; }
        public float? PrecoProd { get; set; }
        public string? Categoria { get; set; }
        public IFormFile? ImgArquivo { get; set; }
        public string? UrlImg { get; set; }
        public string? IdUser { get; set; }
        public string? IdProduto { get; set; }
        //funçao especifica
        public float? ValorTotal { get; set; }
        public int? Qtd { get; set; }
        public int? QtdPorProd { get; set; }
        public string? ErroProd { get; set; }
        public string? Data { get; set; }
        public bool? Cancelado { get; set; }
        public bool? Enviado { get; set; }
        public bool? Recebido { get; set; }
        public string? Fabricante { get; set; }
        public string? Path { get; set; }
    }
    public class ModelNotaFiscal
    {
        public string? IdNota { get; set; }
        public DateTime? Registro { get; set; }
        public ModelDestinatario Destinatario { get; set;}
        public ModelCartao? Cartao { get; set; }
        public List<ModelProduto>? Produto { get; set; }
        public ModelEndereco? Endereco { get; set; }
    }
    public class ModelDestinatario
    {
      
        public string? Nome { get; set; }
        public string? Data { get; set; }
    }
}
