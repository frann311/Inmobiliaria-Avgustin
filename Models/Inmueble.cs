
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_Avgustin.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo Tipo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Tipo válido.")]
        public int TipoId { get; set; }

        [Required(ErrorMessage = "El campo Uso es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Uso válido.")]
        public int UsoId { get; set; }

        [Range(1, 100, ErrorMessage = "Debe tener al menos 1 ambiente.")]
        public int Ambientes { get; set; }

        [Required(ErrorMessage = "Debe ingresar las coordenadas")]
        public int Coordenadas { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La superficie debe ser mayor a 0.")]
        public int Superficie { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public double Precio { get; set; }

        public bool Disponible { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Propietario válido.")]
        public int PropietarioId { get; set; }

        public Propietario Propietario { get; set; }
        public TiposInmuebles Tipo { get; set; }
        public UsosInmuebles Uso { get; set; }
    }


}