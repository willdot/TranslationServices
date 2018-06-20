using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Services.IServices;

namespace Services.Services
{
    public class AmazonUploaderService : IAmazonUploader
    {
        private readonly IAmazonS3 _amazonS3Client; 
        private string _bucketName = "speechtotextcomparison";
        private static readonly string BucketSubdirectory = String.Empty;

        public AmazonUploaderService(IAmazonS3 amazonS3Client)
        {
            _amazonS3Client = amazonS3Client;
        }

        public async Task<bool> CheckBucketExists(string bucketName)
        {
            try
            {
                var res = await _amazonS3Client.DoesS3BucketExistAsync(bucketName);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when searching for bucket", ex.Message);
            }

            return false;
        }

        public async Task<S3UploadResponse> UploadBase64Wav(string base64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                var filename = "S2C" + DateTime.UtcNow.ToString("ddMMyyyyhhmmss") + ".wav";
                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = filename
                };

                using (var ms = new MemoryStream(bytes))
                {
                    request.InputStream = ms;
                    await _amazonS3Client.PutObjectAsync(request);
                    return new S3UploadResponse()
                    {
                        BucketName = _bucketName,
                        FileRoute = filename,
                        BucketRegion = _amazonS3Client.Config.RegionEndpoint.SystemName
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
            }

            return null;
        }

        public async Task<bool> DeleteFile(string filename)
        {
            try
            {
                var res = await _amazonS3Client.DeleteObjectAsync(_bucketName, filename);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public class S3UploadResponse
        {
            public string BucketName { get; set; }
            public string FileRoute { get; set; }
            public string BucketRegion { get; set; }
        }
    }
}
