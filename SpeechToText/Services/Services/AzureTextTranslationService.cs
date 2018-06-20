using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.IServices;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AzureTextTranslationService : IAzureTextTranslationService
    {

        public TextTranslationResult Result { get; set; }

        private List<string> _languages;

        public List<string> Languages
        {
            get { return _languages; }
            set
            {
                if (value != null)
                {
                    _languages = value;
                }
            }
        }

        private readonly string host = "https://api.cognitive.microsofttranslator.com";
        private readonly string path = "/translate?api-version=3.0";

        private readonly string _azureKey;

        private readonly IHttpProxyClientService _httpProxyClientService;
        private readonly IOptions<MyConfig> _config;

        public AzureTextTranslationService()
        {
            this.Result = new TextTranslationResult();
            this.Languages = new List<string>();
        }

        public AzureTextTranslationService(IHttpProxyClientService httpProxyClientService, IOptions<MyConfig> config) : this()
        {
            _httpProxyClientService = httpProxyClientService;
            _config = config;
            _azureKey = _config.Value.AzureTextTranslationKey;
        }

        public async Task<TextTranslationResult> TranslateText(string inputText, List<string> languages)
        {
            this.Languages = languages;

            Object[] body = new Object[] { new { Text = inputText } };
            string requestBody = JsonConvert.SerializeObject(body);

            TextTranslationResult result = new TextTranslationResult();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (HttpClient client = _httpProxyClientService.CreateHttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(this.BuildUri());
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _azureKey);

                    HttpResponseMessage response = await client.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    result.JSONResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);
                }   
            }
            sw.Stop();
            result.ExternalServiceTimeInMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }

        private string BuildUri()
        {
            string uri = host + path;
            foreach (string x in this.Languages)
            {
                uri += $"&to={x}";
            }
            return uri;
        }
    }
}