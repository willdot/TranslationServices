using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.Extensions.Options;
using Services.IServices;
using Services.Models;

namespace Services.Services
{
    public class AwsService : IAwsService
    {

        private readonly IAmazonUploader _amazonUploaderService;
        private readonly IAmazonTranscribeService _amazonTranscribeService;
        private readonly IHttpProxyClientService _httpProxyClientService;

        public AwsService(IAmazonUploader amazonUploader, IAmazonTranscribeService amazonTranscribeService, IHttpProxyClientService httpProxyClientService)
        {
            _amazonUploaderService = amazonUploader;
            _amazonTranscribeService = amazonTranscribeService;
            _httpProxyClientService = httpProxyClientService;
        }

        public async Task<SpeechRecognitionResult> ParseSpeectToText(string[] args)
        {
            if (string.IsNullOrEmpty(args[0])) return null;

            var uploadDetails = await _amazonUploaderService.UploadBase64Wav(args[0]);
            if (string.IsNullOrEmpty(uploadDetails.FileRoute)) return null;

            var transciptionJobName = "Transcribe_" + uploadDetails.FileRoute;
            var request = new StartTranscriptionJobRequest()
            {
                Media = new Media()
                {
                    MediaFileUri = "https://s3." + uploadDetails.BucketRegion + ".amazonaws.com/" + uploadDetails.BucketName + "/" + uploadDetails.FileRoute
                },
                LanguageCode = new LanguageCode(LanguageCode.EnUS),
                MediaFormat = new MediaFormat("Wav"),
                TranscriptionJobName = transciptionJobName
            };

            try
            {
                var res = await _amazonTranscribeService.StartTranscriptionJobAsync(request);

                var jobComplete = false;
                GetTranscriptionJobResponse jobRes = null;
                while (!jobComplete)
                {
                    jobRes = await _amazonTranscribeService.GetTranscriptionJobAsync(new GetTranscriptionJobRequest()
                    {
                        TranscriptionJobName = transciptionJobName
                    });

                    if (jobRes != null && jobRes.TranscriptionJob.TranscriptionJobStatus !=
                        TranscriptionJobStatus.COMPLETED)
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        jobComplete = true;
                    }
                }

                var jsonRes = "";
                using (var client = _httpProxyClientService.CreateHttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = client
                    .GetAsync(jobRes.TranscriptionJob.Transcript.TranscriptFileUri).Result;

                    if (!response.IsSuccessStatusCode) return null;
                    jsonRes = response.Content.ReadAsStringAsync().Result;
                }

                // Once done delete the file
                await _amazonUploaderService.DeleteFile(uploadDetails.FileRoute);

                return new SpeechRecognitionResult()
                {
                    StatusCode = 200,
                    JSONResult =jsonRes
                };

            }
            catch (AmazonTranscribeServiceException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }

    }
}