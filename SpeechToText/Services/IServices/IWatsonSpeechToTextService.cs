using Services.Models;

namespace Services.IServices
{
    public interface IWatsonSpeechToTextService
    {
        SpeechRecognitionResult ParseSpeectToText(string[] args);
    }
}
