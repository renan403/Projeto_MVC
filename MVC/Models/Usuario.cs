using System.ComponentModel.DataAnnotations;
namespace MVC.Models
{
    public class Usuario
    {
        [Key]
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
    }
}

