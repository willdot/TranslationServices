using System.Threading.Tasks;
using Services.Services;

namespace Services.IServices
{
    public interface IAmazonUploader
    {
        Task<AmazonUploaderService.S3UploadResponse> UploadBase64Wav(string base64);
        Task<bool> DeleteFile(string filename);
    }
}
