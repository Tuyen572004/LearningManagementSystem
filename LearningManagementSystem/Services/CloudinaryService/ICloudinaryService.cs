using System.Threading.Tasks;

namespace LearningManagementSystem.Services.CloudinaryService
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(string filePath);
        Task DownloadFileAsync(string fileUri, string targetFilePath);
        Task DeleteFileByUriAsync(string fileUri);
    }
}
