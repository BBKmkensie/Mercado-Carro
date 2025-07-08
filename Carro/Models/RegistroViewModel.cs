using System.ComponentModel.DataAnnotations;

namespace Carro.Models
{
    public class RegistroViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Correo { get; set; }

    }
}
