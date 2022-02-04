using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopifySharp;
using static AR_Holdings.Models.Shopify;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoice _Invoice;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoice Invoice, ILogger<InvoicesController> logger)
        {
            _Invoice = Invoice;
            _logger = logger;
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
        public void Post([FromBody] Object order)
        {
            try
            {
                ShopifyOrder deserializedOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopifyOrder>(order.ToString());
                _Invoice.SaveInvoice(deserializedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                //throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }
    }
}
