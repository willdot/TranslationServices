namespace Services.Models
{
    public class SpeechRecognitionResult
    {
        public int StatusCode { get; set; }
        public string JSONResult { get; set; }
        public long ExternalServiceTimeInMilliseconds { get; set; }
        public long TotalBackendTimeInMilliseconds { get; set; }
    }
}