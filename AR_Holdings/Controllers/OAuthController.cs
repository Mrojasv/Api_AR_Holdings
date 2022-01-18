using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AR_Holdings.Services;
using AR_Holdings.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;
using ShopifySharp.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AR_Holdings.Controllers
{
    [Route("api/[controller]")]
    public class OAuthController : Controller
    {

        private readonly IAuthetication _Authetication;

        public OAuthController(IAuthetication Authetication)
        {
            _Authetication = Authetication;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                await _Authetication.SetAuthorizationAsync();

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
