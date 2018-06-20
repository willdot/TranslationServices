using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.IServices;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class WatsonTextTranslationService : IWatsonTextTranslationService
    {
        private readonly IHttpProxyClientService _httpProxyClientService;
        private readonly IOptions<MyConfig> _config;

        private readonly string host = "https://gateway.watsonplatform.net";
        private readonly string path = "/language-translator/api/v2/translate";

        private static string _username;
        private static string _password;

        public WatsonTextTranslationService(IHttpProxyClientService httpProxyClientService, IOptions<MyConfig> config)
        {
            _httpProxyClientService = httpProxyClientService;
            _config = config;

            _username = _config.Value.WatsonUsernameTextTranslation;
            _password = _config.Value.WatsonPasswordTextTranslation;
        }

        public async Task<TextTranslationResult> TranslateText(string inputText, string inputLanguage, List<string> languages)
        {

            TextTranslationResult result = new TextTranslationResult();

            List<string> jsonTranslationResults = new List<string>();

            foreach (string outputLanguage in languages)
            {
                string translation = await Task.Run(() => this.SingleTranslation(inputText, inputLanguage, outputLanguage.Substring(0, 2)));
                if (!string.IsNullOrEmpty(translation))
                {
                    jsonTranslationResults.Add(translation);
                }
            }

            result.JSONResult = this.GetStringResult(jsonTranslationResults);
            return result;
        }

        // Since watson requires multiple calls to the service, per language, it's necessary to take each JSON result and put it into a string representing an array of json objects
        private string GetStringResult(List<string> input)
        {
            string result = "[\r\n";

            foreach (String x in input)
            {
                result += x + ",";
            }

            result = result.Remove(result.Length - 1);

            result += "\r\n]";

            return result;
        }

        private async Task<string> SingleTranslation(string inputText, string inputLanguage, string outputLanguage)
        {
            string result = "";

            WatsonTranslationBody body = new WatsonTranslationBody()
            {
                text = inputText,
                source = inputLanguage.Substring(0, 2),
                target = outputLanguage
            };

            var jsonBody = JsonConvert.SerializeObject(body);

            using (HttpClient client = _httpProxyClientService.CreateHttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_username + ":" + _password)));
                    request.RequestUri = new Uri(host + path);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (responseBody == null)
                    {
                        return result;
                    }
                    result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);
                }
            }
            return result;
        }
    }
}
