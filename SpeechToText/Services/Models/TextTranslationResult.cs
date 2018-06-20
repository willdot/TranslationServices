using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class TextTranslationResult
    {
        public string DetectedText { get; set; }
        public int StatusCode { get; set; }
        public string JSONResult { get; set; }
        public long ExternalServiceTimeInMilliseconds { get; set; }
        public long TotalBackendTimeInMilliseconds { get; set; }

    }
}
