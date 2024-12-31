using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearningManagementSystem.Services.ConfigService;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services.CloudinaryService
{
    /// <summary>
    /// Configuration settings for Cloudinary.
    /// </summary>
    public class CloudinaryConfig
    {
        /// <summary>
        /// Gets or sets the Cloudinary cloud name.
        /// </summary>
        public string CloudName { get; set; }

        /// <summary>
        /// Gets or sets the Cloudinary API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the Cloudinary API secret.
        /// </summary>
        public string ApiSecret { get; set; }
    }
    /// <summary>
    /// Service for interacting with Cloudinary.
    /// </summary>
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudinaryService"/> class.
        /// </summary>
        /// <param name="configService">The configuration service to retrieve Cloudinary settings.</param>
        public CloudinaryService(IConfigService configService)
        {
            var config = configService.GetCloudinaryConfig();
            Account account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Uploads a file to Cloudinary asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to upload.</param>
        /// <returns>The URL of the uploaded file.</returns>
        public async Task<string> UploadFileAsync(string filePath)
        {
            var uploadParams = new AutoUploadParams
            {
                File = new FileDescription(filePath)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        /// <summary>
        /// Downloads a file from Cloudinary asynchronously.
        /// </summary>
        /// <param name="fileUri">The URI of the file to download.</param>
        /// <param name="targetFilePath">The path to save the downloaded file.</param>
        public async Task DownloadFileAsync(string fileUri, string targetFilePath)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(fileUri);
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Extracts the public ID from a Cloudinary file URI.
        /// </summary>
        /// <param name="fileUri">The URI of the file.</param>
        /// <returns>The public ID of the file.</returns>
        private static string extractPublicId(string fileUri)
        {
            int lastSlashIndex = fileUri.LastIndexOf('/');
            return fileUri.Substring(lastSlashIndex + 1);
        }

        /// <summary>
        /// Deletes a file from Cloudinary by its URI asynchronously.
        /// </summary>
        /// <param name="fileUri">The URI of the file to delete.</param>
        public async Task DeleteFileByUriAsync(string fileUri)
        {
            string publicId = extractPublicId(fileUri);

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);
        }
    }

}
