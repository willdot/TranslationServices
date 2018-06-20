using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services.Models;

namespace STTRest.Controllers
{
    [Route("api/speechToText")]
    public class SpeechToTextController : Controller
    {

        private readonly IBingSpeechService _bingSpeechService;
        private readonly IWatsonSpeechToTextService _watsonSpeechToTextService;
        //private readonly IAwsService _awsService;

        public SpeechToTextController(IBingSpeechService bingSpeechService, IWatsonSpeechToTextService watsonSpeechToTextService /*, IAwsService awsService*/)
        {
            _bingSpeechService = bingSpeechService;
            _watsonSpeechToTextService = watsonSpeechToTextService;
           // _awsService = awsService;

        }

        [HttpPost]
        [Route("parseAzure")]
        public IActionResult ParseSpeectToTextAzure([FromBody] ClientWavObject clientInput)
        {
           // return Ok();
            var sw = new Stopwatch();
            sw.Start();
            
            //Language can be set in the url
            var input = new[]{"https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=en-GB&format=detailed", clientInput.Base64String};
            var result = _bingSpeechService.ParseSpeectToText(input);

            if (result == null)
            {
                return BadRequest("An unknown error has occured");
            }
            
            sw.Stop();
            result.TotalBackendTimeInMilliseconds = sw.ElapsedMilliseconds;
            return Ok(result);
        }
        
        [HttpPost]
        [Route("parseWatson")]
        public IActionResult ParseSpeectToTextWatson([FromBody] ClientWavObject clientInput)
        {

            var sw = new Stopwatch();
            sw.Start();
            
            var input = new string[] {clientInput.Base64String};
            var result = _watsonSpeechToTextService.ParseSpeectToText(input);

            if (result == null)
            {
                return BadRequest("An unknown error has occured");
            }
            
            sw.Stop();
            result.TotalBackendTimeInMilliseconds = sw.ElapsedMilliseconds;
            return Ok(result);
        }

        /*[HttpPost]
        [Route("parseAWS")]
        public async Task<IActionResult> ParseSpeectToTextAWS([FromBody] ClientWavObject clientInput)
        {

            var sw = new Stopwatch();
            sw.Start();

            var input = new string[] { clientInput.Base64String };
            var result = await _awsService.ParseSpeectToText(input);

            if (result == null)
            {
                return BadRequest("An unknown error has occured");
            }

            sw.Stop();
            result.TotalBackendTimeInMilliseconds = sw.ElapsedMilliseconds;
            return Ok(result);
        }*/
    }
}