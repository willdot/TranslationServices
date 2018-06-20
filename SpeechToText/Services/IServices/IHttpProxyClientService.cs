using System.Net;
using System.Net.Http;
using System.Net.WebSockets;

namespace Services.IServices
{
    public interface IHttpProxyClientService
    {
        HttpClient CreateHttpClient();
        HttpWebRequest CreateHttpWebRequest(string requestUri);

		ClientWebSocket CreateClientWebSocket();

	}
}
