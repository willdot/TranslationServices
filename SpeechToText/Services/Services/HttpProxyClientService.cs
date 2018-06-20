using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using Microsoft.Extensions.Options;
using Services.IServices;
using Services.Models;

namespace Services.Services
{
    public class HttpProxyClientService : IHttpProxyClientService
    {
        private readonly IOptions<MyConfig> _config;

        public HttpProxyClientService(IOptions<MyConfig> config)
        {
            _config = config;
        }

        public HttpClient CreateHttpClient()
        {
            try
            {
                var useProxy = _config.Value.UseProxy;
                if (!useProxy)
                {
                    return new HttpClient();
                }

                var proxyHost = _config.Value.ProxyHost;
                var proxyPort = _config.Value.ProxyPort.ToString();

                var proxy = new WebProxy()
                {
                    Address = new Uri("http://" + proxyHost + ":" + proxyPort),
                    UseDefaultCredentials = true
                };

                var httpClientHandler = new HttpClientHandler()
                {
                    Proxy = proxy,
                };

                var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }

		public ClientWebSocket CreateClientWebSocket()
		{
			var useProxy = _config.Value.UseProxy;
			if (!useProxy)
			{
				return new ClientWebSocket();
			}

			var proxyHost = _config.Value.ProxyHost;
			var proxyPort = _config.Value.ProxyPort.ToString();

			try
			{
				var res = new ClientWebSocket();

				res.Options.Proxy = new WebProxy()
				{
					Address = new Uri("http://" + proxyHost + ":" + proxyPort),
					UseDefaultCredentials = true,
					BypassProxyOnLocal = false
				};

				return res;
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			return null;
		}

		public HttpWebRequest CreateHttpWebRequest(string requestUri)
        {
            try
            {
                var useProxy = _config.Value.UseProxy;
                HttpWebRequest res = (HttpWebRequest)WebRequest.Create(requestUri);

                if (!useProxy)
                {
                    return res;
                }

                var proxyHost = _config.Value.ProxyHost;
                var proxyPort = _config.Value.ProxyPort;
                var myproxy = new WebProxy(proxyHost, proxyPort) { BypassProxyOnLocal = false };
                res.Proxy = myproxy;
                
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }
	}
}
