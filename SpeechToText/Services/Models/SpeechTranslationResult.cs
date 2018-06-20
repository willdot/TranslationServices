using System;
namespace Services.Models
{
    public class SpeechTranslationResult
    {
        public int StatusCode { get; set; }
        public string JSONResult { get; set; }
        public long ExternalServiceTimeInMilliseconds { get; set; }
        public long TotalBackendTimeInMilliseconds { get; set; }
    }
}
