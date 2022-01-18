using System;
namespace AR_Holdings.Utilities
{
    public static class Settings
    {
        public static ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public static General General { get; set; } = new General();
    }

    public class General
    { 
        public string ShopifyUrl { get; set; }
        public string ShopifyApiKey { get; set; }
        public string ShopifySecretKey { get; set; }
        public string RedirectURL { get; set; }
        public string ShopAccessToken { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
