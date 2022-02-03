using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AR_Holdings.Models
{
        [Table("ColaArticulos")]
        public class ColaArticulos
        {
            [Key]
            public int ID { get; set; }
            [Required]
            public string SKU { get; set; }
            [Required]
            public int? Cantidad { get; set; }
            [Required]
            public DateTime FechaRegistro { get; set; }
            public DateTime FechaActualizacion { get; set; }
            public bool Sincronizado { get; set; }
        }
   
}
