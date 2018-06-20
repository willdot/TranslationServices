using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Services.IServices;
using Services.Models;

namespace STTRest.Controllers
{
    [Produces("application/json")]
    [Route("api/SpeechTranslation")]
    public class SpeechTranslationController : Controller
    {
		private readonly IAzureSpeechTranslatorService _azureSpeechTranslatorService;
        private readonly IAzureTextTranslationService _azureTextTranslatorService;
        private readonly IBingSpeechService _bingSpeechService;
        private readonly IWatsonTextTranslationService _watsonTextTranslationService;
        private readonly IWatsonSpeechToTextService _watsonSpeechToTextService;

        public SpeechTranslationController(IAzureSpeechTranslatorService azureSpeechTranslatorService,
            IAzureTextTranslationService azureTextTranslatorSevice,
            IBingSpeechService bingSpeechService,
            IWatsonTextTranslationService watsonTranslationService,
            IWatsonSpeechToTextService watsonSpeechToTextService)
		{
			_azureSpeechTranslatorService = azureSpeechTranslatorService;
            _azureTextTranslatorService = azureTextTranslatorSevice;
            _bingSpeechService = bingSpeechService;
            _watsonTextTranslationService = watsonTranslationService;
            _watsonSpeechToTextService = watsonSpeechToTextService;
        }

		[HttpGet]
		[Route("test")]
		public async Task<IActionResult> TestAzureTranslateAsync()
		{
            //_azureSpeechTranslatorService.TranslateSpeech("speak.wav");
            List<string> a = new List<string>() { "fr", "es" };
            //TextTranslationResult result = await Task.Run(() => _watsonTextTranslationService.TranslateText("Hello", "en", a));
            TextTranslationResult result = await Task.Run(() => _azureTextTranslatorService.TranslateText("Good morning", a));
            
			return Ok();
		}

        [HttpPost]
        [Route("parseAzure")]
        public async Task<IActionResult> AzureTranslate([FromBody] ClientTranslationInput clientInput)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Language can be set in the url
            string[] input = new[] { $"https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language={clientInput.InputLanguage}&format=detailed", clientInput.Base64String };
            SpeechRecognitionResult textResult = _bingSpeechService.ParseSpeectToText(input);

            string detectedText = "";

            try
            {
                JObject json = JObject.Parse(textResult.JSONResult);
                detectedText = json["NBest"][0]["Display"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (string.IsNullOrEmpty(detectedText))
            {
                sw.Stop();
                return BadRequest("No text detected");
            }

            TextTranslationResult result = await Task.Run(() => _azureTextTranslatorService.TranslateText(detectedText, clientInput.OutputLanguages.ToList()));
            
            if (result == null)
            {
                sw.Stop();
                return BadRequest("An unknown error has occured");
            }

            sw.Stop();
            result.TotalBackendTimeInMilliseconds = sw.ElapsedMilliseconds;
            result.DetectedText = detectedText;
            return Ok(result);
        }

        [HttpPost]
        [Route("parseWatson")]
        public async Task<IActionResult> WatsonTranslate([FromBody] ClientTranslationInput clientInput)
        {
            var input = new string[] { clientInput.Base64String };
            Stopwatch sw = new Stopwatch();
            sw.Start();

            SpeechRecognitionResult textResult = _watsonSpeechToTextService.ParseSpeectToText(input);

            string detectedText = "";

            try
            {
                JObject json = JObject.Parse(textResult.JSONResult);
                detectedText = json["results"][0]["alternatives"][0]["transcript"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (string.IsNullOrEmpty(detectedText))
            {
                sw.Stop();
                return BadRequest("No text detected");
            }

            TextTranslationResult result = await Task.Run(() => _watsonTextTranslationService.TranslateText(detectedText, clientInput.InputLanguage, clientInput.OutputLanguages.ToList()));

            if (result == null)
            {
                sw.Stop();
                return BadRequest("An unknown error has occured");
            }

            sw.Stop();
            result.TotalBackendTimeInMilliseconds = sw.ElapsedMilliseconds;
            result.DetectedText = detectedText;
            return Ok(result);
        }


    }
}