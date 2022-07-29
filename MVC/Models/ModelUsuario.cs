using System.ComponentModel.DataAnnotations;
namespace MVC.Models
{
    public class ModelUsuario
    {

        [Required(ErrorMessage = "Insira seu nome")]
        [StringLength(100, ErrorMessage = "Campo Login: aceita no máximo 100 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Digite seu e-mail")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
        ErrorMessage = "Email inválido.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Senha deve conter no mínimo 8 caracteres")]
        [MinLength(8, ErrorMessage = "O campo {0} deve ter no mínimo {1} caracteres")]
        public string? Senha { get; set; }
        public string? ConfSenha { get; set; }
        public string? Message { get; set; }
        public string? IdUser { get; set; }
        public string? Erro { get; set; }

    }
    public class ModelEndereco
    {
        [Required]
        public string? Pais { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Campo Login: aceita no máximo 100 caracteres.")]
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        [Required]
        [MinLength(9, ErrorMessage = "O campo {0} deve ter no mínimo {1} caracteres")]
        [MaxLength(9, ErrorMessage = "Cep Invalido")]
        public string? Cep { get; set; }
        [Required]
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        [Required]
        public int Numero { get; set; }
        [Required]
        public string? Bairro { get; set; }
        [Required]
        public string? Cidade { get; set; }
        [Required]
        public string? Estado { get; set; }
        public bool Padrao { get; set; }
        public string? UF { get; set; }
    }
    
    public class ModelCartao
    {
        public string? NomeCard { get; set; }
        public string? NumeroCard { get; set; }
        public string? DataExpiracao { get; set; }
        public int? Cvv { get; set; }
        public string? Bandeira { get; set; }
        public string? Erro { get; set; } = string.Empty;
        public string? CaminhoImgBandeira { get; set; }
        public string? CaminhoImgCartao { get; set; }
        public bool Padrao { get; set; }
        public string? QuatroDigCard { get; set; }
    }
}
