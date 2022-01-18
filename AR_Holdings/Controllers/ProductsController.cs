using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ISynchronize _Synchronize;

        public ProductsController(ISynchronize Synchronize)
        {
            _Synchronize = Synchronize;
        }

        [HttpGet]
        public async Task<string> ProductsShopifyAsync()
        {
            try
            {
                await _Synchronize.ProductsShopifyAsync();

                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return "OK";
            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }

        
    }
}
