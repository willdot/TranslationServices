using Services.Models;

namespace Services.IServices
{
    public interface IBingSpeechService
    {
        SpeechRecognitionResult ParseSpeectToText(string[] args);
    }
}
