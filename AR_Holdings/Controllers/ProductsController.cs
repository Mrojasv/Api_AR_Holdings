using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ISynchronize _Synchronize;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ISynchronize Synchronize, ILogger<ProductsController> logger)
        {
            _Synchronize = Synchronize;
            _logger = logger;
        }

        [HttpGet("LoadShopifyAsync")]
        public async Task<string> LoadShopifyAsync()
        {
            try
            {
                await _Synchronize.LoadShopifyAsync();

                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return "COMPLETED";
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }

        [HttpGet("ProductsShopifyAsync")]
        public async Task<string> ProductsShopifyAsync()
        {
            try
            {
                await _Synchronize.ProductsShopifyAsync();

                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return "COMPLETED";
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }

        [HttpGet("ShopifyAsync")]
        public async Task<string> ShopifyAsync()
        {
            try
            {
                await _Synchronize.ShopifyAsync();

                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return "COMPLETED";
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                throw new Exception("Ocurrió un error inesperado, intentelo nuevamente.");
            }
        }
    }
}
