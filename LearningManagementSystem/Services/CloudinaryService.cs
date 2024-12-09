using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LearningManagementSystem.Services
{
    public class CloudinaryConfig
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            Account account = Initialize();
            _cloudinary = new Cloudinary(account);
        }

        private Account Initialize()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            string configContent = File.ReadAllText(configFilePath);
            var configJson = JsonDocument.Parse(configContent);
            var cloudinaryConfig = configJson.RootElement.GetProperty("Cloudinary").ToString();
            CloudinaryConfig config = JsonSerializer.Deserialize<CloudinaryConfig>(cloudinaryConfig);

            var account = new Account(
                config.CloudName,
                config.ApiKey,
                config.ApiSecret
            );

            return account;
        }

        public async Task<string> UploadFileAsync(string filePath)
        {
            var uploadParams = new AutoUploadParams
            {
                File = new FileDescription(filePath)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }



        // Download file using the URI from Cloudinary
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
                //Console.WriteLine($"An error occurred while downloading the file: {ex.Message}");
                throw new Exception($"An error occured while downloading the file: {ex.Message}");
            }
        }

        private static string extractPublicId(string fileUri)
        {
            // Find the position of the last '/'
            int lastSlashIndex = fileUri.LastIndexOf('/');

            // Extract the substring from the character after the last '/'
            return fileUri.Substring(lastSlashIndex + 1);
        }

        public async Task DeleteFileByUriAsync(string fileUri)
        {
            // Extract the public ID from the URI
            string publicId = extractPublicId(fileUri);

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok")
            {
                Console.WriteLine($"File successfully deleted: {publicId}");
            }
            else
            {
                Console.WriteLine($"Failed to delete file. Result: {result.Result}");
            }
        }
    }

}
