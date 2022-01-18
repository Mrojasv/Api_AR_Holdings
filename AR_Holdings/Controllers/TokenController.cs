using System;
using System.Threading.Tasks;
using AR_Holdings.Services;
using AR_Holdings.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IAuthetication _Authetication;

        public TokenController(IAuthetication Authetication)
        {
            _Authetication = Authetication;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get(string code, string shop)
        {
            try
            {
                await _Authetication.GetTokenAsync(code, shop);

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
