using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria_Avgustin.Models
{
    public class Contrato : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inmueble válido.")]
        [ForeignKey("Inmueble")]
        public int Id_Inmueble { get; set; }
        public virtual Inmueble Inmueble { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inquilino válido.")]
        [ForeignKey("Inquilino")]
        public int Id_Inquilino { get; set; }
        public virtual Inquilino Inquilino { get; set; }

        [Required(ErrorMessage = "Debe ingresar un monto.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha de inicio.")]
        [DataType(DataType.Date)]
        public DateTime Fecha_Inicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha de fin.")]
        [DataType(DataType.Date)]
        public DateTime Fecha_Fin { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha_Rescision { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Multa { get; set; }


        // AUDITORIA --------------------------------------

        // USUARIO ---------------------------------
        public int UsuarioCreadorId { get; set; }
        public int? UsuarioFinalizadorId { get; set; }

        public Usuario UsuarioCreador { get; set; }
        public Usuario UsuarioFinalizador { get; set; }


        // FECHAS ----------------------------------

        [DataType(DataType.DateTime)]
        public DateTime Creado_En { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime Actualizado_En { get; set; } = DateTime.Now;

        // ✅ Validaciones personalizadas
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha_Inicio.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser menor a hoy.",
                    new[] { nameof(Fecha_Inicio) });
            }

            if (Fecha_Fin.Date < Fecha_Inicio.Date)
            {
                yield return new ValidationResult(
                    "La fecha de fin no puede ser menor que la fecha de inicio.",
                    new[] { nameof(Fecha_Fin) });
            }
        }
    }
}
