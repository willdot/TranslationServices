using Services.IServices;
using Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AzureSpeechTranslatorService: IAzureSpeechTranslatorService
	{
        public SpeechTranslationResult Result 
        {
            get {return this._result; }
            set { this._result = value; }
        }
        private SpeechTranslationResult _result;

        private string host = "wss://dev.microsofttranslator.com";
		private string path = "/speech/translate";

		// NOTE: Replace this example key with a valid subscription key.
		private string key = "d810fbb4182f4fe0afc2656c439c1ef4";

		private readonly IHttpProxyClientService _httpProxyClientService;

        private ClientTranslationInput _input;

        private string _audioInput;

        public AzureSpeechTranslatorService()
        {
            this.Result = new SpeechTranslationResult();
        }

        public AzureSpeechTranslatorService(IHttpProxyClientService httpProxyClientService): this()
		{
			_httpProxyClientService = httpProxyClientService;
		}

        public async Task<SpeechTranslationResult> TranslateSpeech(ClientTranslationInput input)
        {
            _input = input;
            //_audioInput = Convert.ToBase64String(File.ReadAllBytes(_input.Base64String));
            //_audioInput = _input.Base64String;

            Byte[] b = Convert.FromBase64String(input.Base64String);
            System.IO.File.WriteAllBytes(@"speak3.wav", b);
            _audioInput = Convert.ToBase64String(File.ReadAllBytes("speak3.wav"));

            this.Result = await Translate();
            return this.Result;
        }

        private async Task<SpeechTranslationResult> Translate()
        {
            var client = _httpProxyClientService.CreateClientWebSocket();
            client.Options.SetRequestHeader("Ocp-Apim-Subscription-Key", key);

            string from = _input.InputLanguage;//"fr-FR";
            string to = "en-US";
            string voice = _input.OutputVoice;//"en-US-BenjaminRUS";
            string api = "1.0";
            string output_path = "speak2.wav";

            string uri = host + path +
                "?from=" + from +
                "&to=" + to +
                "&api-version=" + api +
                "&voice=" + voice;

            if (_input.ReturnAudioOutput)
            {
                uri += "&features=texttospeech";
            }

            Console.WriteLine("uri: " + uri);
            Console.WriteLine("Opening connection.");
            await client.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connection open.");
            Task.WhenAll(Send(client), Receive(client, output_path)).Wait();

            return this.Result;
        }

		private async Task Send(ClientWebSocket client)
		{
			try
            {
                var audio = Convert.FromBase64String(_audioInput);
                var audio_out_buffer = new ArraySegment<byte>(audio);
				Console.WriteLine("Sending audio.");
				await client.SendAsync(audio_out_buffer, WebSocketMessageType.Binary, true, CancellationToken.None);

				/* Make sure the audio file is followed by silence.
				 * This lets the service know that the audio input is finished. */
				var silence = new byte[3200000];
				var silence_buffer = new ArraySegment<byte>(silence);
				await client.SendAsync(silence_buffer, WebSocketMessageType.Binary, true, CancellationToken.None);

				Console.WriteLine("Done sending.");
				await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private async Task Receive(ClientWebSocket client, string output_path)
		{
			var inbuf = new byte[102400];
			var segment = new ArraySegment<byte>(inbuf);
			var stream = new FileStream(output_path, FileMode.Create);

			Console.WriteLine("Awaiting response.");
			while (client.State == WebSocketState.Open)
			{
				var result = await client.ReceiveAsync(segment, CancellationToken.None);
				switch (result.MessageType)
				{
					case WebSocketMessageType.Close:
						Console.WriteLine("Received close message. Status: " + result.CloseStatus + ". Description: " + result.CloseStatusDescription);
						await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
						break;
					case WebSocketMessageType.Text:
						Console.WriteLine("Received text.");
                        this.Result.JSONResult = Encoding.UTF8.GetString(inbuf).TrimEnd('\0');
						Console.WriteLine(Encoding.UTF8.GetString(inbuf).TrimEnd('\0'));
						break;
					case WebSocketMessageType.Binary:
						Console.WriteLine("Received binary data: " + result.Count + " bytes.");
						stream.Write(inbuf, 0, result.Count);
						break;
				}
                Console.WriteLine(  );
			}

			stream.Close();
			stream.Dispose();
		}
	}
}
