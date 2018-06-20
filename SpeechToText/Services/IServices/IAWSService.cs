using System.Threading.Tasks;
using Services.Models;

namespace Services.IServices
{
    public interface IAwsService
    {
        Task<SpeechRecognitionResult> ParseSpeectToText(string[] args);
    }
}
