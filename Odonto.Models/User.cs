using System.ComponentModel.DataAnnotations;

namespace Odonto.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public bool Active { get; set; } = true;

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Type { get; set; }

        public string TypeName { get; set; }
    }
}
