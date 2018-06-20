using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Services.IServices;
using Services.Models;

namespace Services.Services
{
    public class AzureAuthenticationService : IAzureAuthenticationService
    {
        private readonly IHttpProxyClientService _proxyClientService;
        private readonly IOptions<MyConfig> _config;

        public static readonly string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        private readonly string _subscriptionKey;
        private string _token;
        private readonly Timer _accessTokenRenewer;
        

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public AzureAuthenticationService(IHttpProxyClientService proxyClientService, IOptions<MyConfig> config)
        {
            _proxyClientService = proxyClientService;
            _config = config;

            _subscriptionKey = _config.Value.BingSubscriptionKey;
            _token = FetchToken(FetchTokenUri, _subscriptionKey).Result;

            // renew the token every specfied minutes
            _accessTokenRenewer = new Timer(OnTokenExpiredCallback,
                this,
                TimeSpan.FromMinutes(RefreshTokenDuration),
                TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return _token;
        }

        private void RenewAccessToken()
        {
            _token = FetchToken(FetchTokenUri, _subscriptionKey).Result;
            Console.WriteLine("Renewed token.");
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed renewing access token. Details: {0}", ex.Message);
            }
            finally
            {
                try
                {
                    _accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration),
                        TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message);
                }
            }
        }

        private async Task<string> FetchToken(string fetchUri, string subscriptionKey)
        {
            using (var client = _proxyClientService.CreateHttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                var uriBuilder = new UriBuilder(fetchUri);
                uriBuilder.Path += "/issueToken";

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}