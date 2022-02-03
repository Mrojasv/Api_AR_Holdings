using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AR_Holdings.Models
{
        [Table("Cliente")]
        public class Cliente
        {
            [Key]
            public int ID { get; set; }
            [Required]
            public int OrderId { get; set; }
            [Required]
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
        }
   
}
