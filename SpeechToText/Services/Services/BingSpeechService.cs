using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Services.IServices;
using Services.Models;

namespace Services.Services
{
    internal class BingSpeechService : IBingSpeechService
    {
        // Note: Sign up at https://azure.microsoft.com/en-us/try/cognitive-services/ to get a subscription key.  
        // This can then be assigned within the appsettings.json file under MyConfig > BingSubscriptionKey
        private static IAzureAuthenticationService _azureAuthenticationService;
        private readonly IHttpProxyClientService _httpProxyClientService;

        public BingSpeechService(IAzureAuthenticationService azureAzureAuthenticationService, IHttpProxyClientService httpProxyClientService)
        {
            _azureAuthenticationService = azureAzureAuthenticationService;
            _httpProxyClientService = httpProxyClientService;
        }

        public SpeechRecognitionResult ParseSpeectToText(string[] args)
        {

            var requestUri = args[0];/*.Trim(new char[] { '/', '?' });*/

            const string host = @"speech.platform.bing.com";
            const string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
           
            var audioBase64 = args[1];
            if (string.IsNullOrEmpty(audioBase64)) return null;

            var token = _azureAuthenticationService.GetAccessToken();
            var request = _httpProxyClientService.CreateHttpWebRequest(requestUri);
            request.SendChunked = true;
            request.Accept = @"application/json;text/xml";
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            request.Host = host;
            request.ContentType = contentType;
            request.Headers["Authorization"] = "Bearer " + token;

            var bytes = Convert.FromBase64String(audioBase64);
            using (var memoryStream = new MemoryStream(bytes))
            {
                /*
                * Open a request stream and write 1024 byte chunks in the stream one at a time.
                */
                using (var requestStream = request.GetRequestStream())
                {
                    /*
                    * Read 1024 raw bytes from the input audio file.
                    */
                    var buffer = new byte[checked((uint) Math.Min(1024, (int) memoryStream.Length))];
                    int bytesRead;
                    while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    // Flush
                    requestStream.Flush();
                }

                var sw = new Stopwatch();
                sw.Start();
                    
                using (var response = request.GetResponse())
                {
                    var statusCode = ((HttpWebResponse) response).StatusCode.ToString();
                    int.TryParse(statusCode, out var statusCodeInt);

                    string responseString;
                    using (var sr = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                    {
                        responseString = sr.ReadToEnd();
                    }
                        
                    sw.Stop();
                    return new SpeechRecognitionResult()
                    {
                        StatusCode = statusCodeInt,
                        JSONResult = responseString,
                        ExternalServiceTimeInMilliseconds = sw.ElapsedMilliseconds
                    };
                }   
            }
        }
    }
}
