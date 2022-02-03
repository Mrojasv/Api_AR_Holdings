using System.Collections.Generic;

namespace AR_Holdings.Models
{
    public class Invoices
    {
        public Orden Orden { get; set; }
        public List<Articulos> Articulos { get; set; }
        public Cliente Cliente { get; set; }
    }
}
