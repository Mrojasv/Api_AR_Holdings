using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AR_Holdings.Models
{

        [Table("Orden")]
        public class Orden
        {
            [Key]
            public int ID { get; set; }
            [Required]
            public int NumeroOrden { get; set; }
            [Required]
            public decimal SubTotal { get; set; }
            [Required]
            public decimal TotalImpuestos { get; set; }
            [Required]
            public decimal Total { get; set; }
            [Required]
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaOrden { get; set; }
        }

        [Table("Articulos")]
        public class Articulos
        {
            [Key]
            public int ID { get; set; }
            [Required]
            public int OrderId { get; set; }
            [Required]
            public string SKU { get; set; }
            [Required]
            public decimal Precio { get; set; }
            [Required]
            public int Cantidad { get; set; }
            public string Nombre { get; set; }
            [Required]
            public decimal SubTotal { get; set; }
            [Required]
            public decimal TotalImpuestos { get; set; }
            [Required]
            public decimal Total { get; set; }
        }
   
}
