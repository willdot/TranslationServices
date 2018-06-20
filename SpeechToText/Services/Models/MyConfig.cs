namespace Services.Models
{
    public class MyConfig
    {
        public bool UseProxy { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string BingSubscriptionKey { get; set; }
        public string WatsonUsernameTextTranslation { get; set; }
        public string WatsonPasswordTextTranslation { get; set; }
        public string WatsonUsernameSpeechToText { get; set; }
        public string WatsonPasswordSpeechToText { get; set; }

        public string AzureTextTranslationKey { get; set; }
    }
}
