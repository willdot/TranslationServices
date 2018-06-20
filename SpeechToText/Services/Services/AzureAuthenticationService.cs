using System;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Services.IServices;

namespace Services.Services
{
    internal class AzureAuthenticationService : IAzureAuthetentication
    {
        public static readonly string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        private string _subscriptionKey;
        private string _token;
        private Timer _accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public void Initialise(string subscriptionKey)
        {
            if (_subscriptionKey == null) return;
            _subscriptionKey = subscriptionKey;
            _token = FetchToken(FetchTokenUri, subscriptionKey).Result;

            // renew the token every specfied minutes
            _accessTokenRenewer = new Timer(OnTokenExpiredCallback,
                this,
                TimeSpan.FromMinutes(RefreshTokenDuration),
                TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return string.IsNullOrEmpty(_subscriptionKey) ? "" : _token;
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
            if (string.IsNullOrEmpty(_subscriptionKey))
            {
                return "";
            }

            using (var client = new HttpClient())
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