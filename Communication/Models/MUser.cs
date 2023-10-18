using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Communication.Models
{
    public class MUser : IdentityUser
    {
        public virtual string? Name { get; set; }

        [MinLength(3, ErrorMessage ="Informe ao menos 3 carecteres para o usuario.")]
        [Required(ErrorMessage ="Nome de Usuario obrigatorio.")]
        public override  string UserName { get; set; }


        [MinLength(6, ErrorMessage = "Informe ao menos 6 carecteres para a senha.")]
        [Required(ErrorMessage = "Senha obrigatorio.")]
        public override string PasswordHash { get; set; }
        public List<MFile>?  Files { get; set; }
    }
}
