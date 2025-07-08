using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Framework;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace  Carro.Models
{
    public class ProfileViewModel
    {
        public Guid UsersId { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio!")]

        public string Name { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio!")]

        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio!")]

        public string Username { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio!")]

        public string Rol { get; set; }





    }
}