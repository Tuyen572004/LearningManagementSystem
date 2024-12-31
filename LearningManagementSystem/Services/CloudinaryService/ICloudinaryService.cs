using System.Threading.Tasks;

namespace LearningManagementSystem.Services.CloudinaryService
{
    /// <summary>
    /// Interface for Cloudinary service to handle file operations.
    /// </summary>
    public interface ICloudinaryService
    {
        /// <summary>
        /// Uploads a file to Cloudinary.
        /// </summary>
        /// <param name="filePath">The local path of the file to be uploaded.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the uploaded file.</returns>
        Task<string> UploadFileAsync(string filePath);

        /// <summary>
        /// Downloads a file from Cloudinary.
        /// </summary>
        /// <param name="fileUri">The URI of the file to be downloaded.</param>
        /// <param name="targetFilePath">The local path where the file will be saved.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DownloadFileAsync(string fileUri, string targetFilePath);

        /// <summary>
        /// Deletes a file from Cloudinary by its URI.
        /// </summary>
        /// <param name="fileUri">The URI of the file to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteFileByUriAsync(string fileUri);
    }
}
