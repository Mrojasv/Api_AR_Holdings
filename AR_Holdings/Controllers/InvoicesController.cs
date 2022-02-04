using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.Services;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoice _Invoice;

        public InvoicesController(IInvoice Invoice)
        {
            _Invoice = Invoice;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            throw new NotImplementedException();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Order order)
        {
            try
            {
                _Invoice.SaveInvoice(order);
            }
            catch (Exception ex)
            {
                //throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }
    }
}
