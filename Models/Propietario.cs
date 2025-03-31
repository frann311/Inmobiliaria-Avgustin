using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_Avgustin.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos.")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "El numero de telefono debe tener entre 7 y 8 dígitos.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio.")]
        public string Email { get; set; }
    }
}