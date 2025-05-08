using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria_Avgustin.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }

        [Required]
        public int NumeroPago { get; set; }


        [DataType(DataType.Date)]
        public DateTime? FechaPago { get; set; }

        [StringLength(200)]
        public string Concepto { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Importe { get; set; }

        [Required]
        public bool Anulado { get; set; } = false;


        [DataType(DataType.DateTime)]
        public DateTime Fecha_Vencimiento { get; set; }



        // auditoria -----------------------------------

        public int UsuarioCreadorId { get; set; }
        public int? UsuarioAnuladorId { get; set; }

        public Usuario UsuarioCreador { get; set; }
        public Usuario UsuarioAnulador { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreadoEn { get; set; } = DateTime.Now;


        [DataType(DataType.DateTime)]
        public DateTime? AnuladoEn { get; set; }

    }
}
