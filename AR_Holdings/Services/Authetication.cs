using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AR_Holdings.Utilities;
using ShopifySharp;
using ShopifySharp.Enums;

namespace AR_Holdings.Services
{
    public interface IAuthetication
    {
        Task SetAuthorizationAsync();
        Task GetTokenAsync(string code, string shopifyUrl);
    }

    public class Authetication: IAuthetication
    {
        public async Task SetAuthorizationAsync()
        {
            string usersMyShopifyUrl = Settings.General.ShopifyUrl;
            bool isValidDomain = await AuthorizationService.IsValidShopDomainAsync(usersMyShopifyUrl);

            if (isValidDomain)
            {
                string redirectUrl = Settings.General.RedirectURL;

                //An array of the Shopify access scopes your application needs to run.
                var scopes = new List<AuthorizationScope>()
                {
                    AuthorizationScope.ReadProducts,
                    AuthorizationScope.WriteProducts
                };

                string shopifyApiKey = Settings.General.ShopifyApiKey;

                //All AuthorizationService methods are static.
                Uri authUrl = AuthorizationService.BuildAuthorizationUrl(scopes, usersMyShopifyUrl, shopifyApiKey, redirectUrl);
            }
        }

        public async Task GetTokenAsync(string code, string shopifyUrl)
        {
            string shopifyApiKey = Settings.General.ShopifyApiKey;
            string shopifySecretKey = Settings.General.ShopifySecretKey;

            string accessToken = await AuthorizationService.Authorize(code, shopifyUrl, shopifyApiKey, shopifySecretKey);
        }
    }
}
