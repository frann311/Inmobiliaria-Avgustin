using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_Avgustin.Models
{
    public class Inquilino
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El Nombre no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El Apellido no puede tener más de 50 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe contener entre 7 y 8 dígitos numéricos.")]
        public string Dni { get; set; }
        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "El número de teléfono debe contener entre 7 y 15 dígitos.")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del Email no es válido.")]
        public string Email { get; set; }
        [StringLength(100, ErrorMessage = "El Trabajo no puede tener más de 100 caracteres.")]
        public string Trabajo { get; set; }
        [Required(ErrorMessage = "El monto de ingresos es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "Los ingresos deben ser un número positivo.")]
        public int Ingresos { get; set; }

    }
}