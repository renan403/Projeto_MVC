using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class ModelLogin
    {

        [Required(ErrorMessage = "Digite seu e-mail")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
        ErrorMessage = "Email inválido.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public bool? Ativo { get; set; }
        public string? Nome { get; set; }
        public string? Resposta { get; set; }
    }
}
