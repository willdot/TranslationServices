using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Services.IServices;
using Services.Models;

namespace Services.Services
{
    public class WatsonSpeechToTextService : IWatsonSpeechToTextService
    {
        private readonly IHttpProxyClientService _httpProxyClientService;
        private readonly IOptions<MyConfig> _config;

        private static string _username;
        private static string _password; 
        private static string _model = "en-UK_NarrowbandModel";

        public WatsonSpeechToTextService(IHttpProxyClientService httpProxyClientService, IOptions<MyConfig> config)
        {
            _httpProxyClientService = httpProxyClientService;
            _config = config;

            _username = _config.Value.WatsonUsernameSpeechToText;
            _password = _config.Value.WatsonPasswordSpeechToText;
        }

        public SpeechRecognitionResult ParseSpeectToText(string[] args)
        {
            var base64String = args[0];
            using (var client = _httpProxyClientService.CreateHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                            _username + ":" + _password)));
    
                var bytes = Convert.FromBase64String(base64String);
                var content = new StreamContent(new MemoryStream(bytes));
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                Stopwatch sw = new Stopwatch();
                sw.Start();

                //Language can be set in the url
                var response = client
                    .PostAsync("https://stream.watsonplatform.net/speech-to-text/api/v1/recognize?continuous=true&_model=" + _model,
                        content).Result;

                if (!response.IsSuccessStatusCode) return null;
                var res = response.Content.ReadAsStringAsync().Result;
                
                sw.Stop();
                
                return new SpeechRecognitionResult()
                {
                    JSONResult = res,
                    StatusCode = 200,
                    ExternalServiceTimeInMilliseconds = sw.ElapsedMilliseconds
                };
            }
        }
    }
}