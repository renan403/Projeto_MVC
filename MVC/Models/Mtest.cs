using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MVC.Models
{
    public class Mtest
    {
        [Key]
        public string? Nome { get; set; }
        
    }
    
}
