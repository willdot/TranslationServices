using Amazon.S3;
using Amazon.TranscribeService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Services.IServices;
using Services.Models;
using Services.Services;

namespace Services
{
    public static class Startup
    {

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBingSpeechService, BingSpeechService>();
            services.AddTransient<IWatsonSpeechToTextService, WatsonSpeechToTextService>();
            //services.AddTransient<IAwsService, AwsService>();

            //services.AddTransient<IAmazonUploader, AmazonUploaderService>();
            services.AddTransient<IAzureAuthenticationService, AzureAuthenticationService>();

			services.AddTransient<IAzureSpeechTranslatorService, AzureSpeechTranslatorService>();
            services.AddTransient<IAzureTextTranslationService, AzureTextTranslationService>();
            services.AddTransient<IWatsonTextTranslationService, WatsonTextTranslationService>();

            services.AddTransient<IHttpProxyClientService, HttpProxyClientService>();

            var x = configuration.GetAWSOptions();
            //services.AddDefaultAWSOptions(configuration.GetAWSOptions());
           // services.AddAWSService<IAmazonS3>();
           // services.AddAWSService<IAmazonTranscribeService>();

        }

    }
}
