// Models/Imagen.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Inmobiliaria_Avgustin.Models
{
    public class Imagen
    {
        [Key]
        public int Id { get; set; }

        // Clave foránea al inmueble
        [Required]
        public int InmuebleId { get; set; }

        // Aquí guardaremos la URL relativa (p.ej. "/Uploads/Inmuebles/5/archivo.jpg")
        [Required]
        public string Url { get; set; }

        // Nuevo campo para marcar la portada
        // TINYINT(1) en MySQL se mapea a bool en C#
        public bool EsPortada { get; set; } = false;

        // No mapea a BD: es el archivo subido en el form
        [Display(Name = "Seleccionar imágenes")]
        public IFormFile? Archivo { get; set; }
    }
}
