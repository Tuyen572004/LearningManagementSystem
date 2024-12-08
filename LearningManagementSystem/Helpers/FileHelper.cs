using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Microsoft.UI.Xaml; // For WinUI window
using WinRT.Interop;
using Windows.Storage.Pickers; // For InitializeWithWindow


namespace LearningManagementSystem.Helpers
{
    public class FileHelper
    {

        public async Task<StorageFile> ChooseFile()
        {
            try
            {
                // Create the file picker
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };
                picker.FileTypeFilter.Add("*");
                
                

                // Associate the picker with the current window
                InitializeWithWindow.Initialize(picker, DashBoard.HWND);

                // Show the file picker
                return await picker.PickSingleFileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting file: {ex.Message}");
                return null;
            }
        }


        public async Task SaveFile(StorageFile file)
        {
            try
            {
                string targetFolderPath = "D:\\Files\\Submissions";
                // Ensure the target folder exists
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                // Build the full path for the file
                string targetFilePath = Path.Combine(targetFolderPath, file.Name);

                // Read the file content and write it to the target location
                using (var inputStream = await file.OpenStreamForReadAsync())
                using (var outputStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputStream.CopyToAsync(outputStream);
                }

                Console.WriteLine($"File saved to: {targetFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }

        public async Task<StorageFolder> ChooseFolder()
        {
            // Open folder picker to select a directory
            var picker = new FolderPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            // Add a wildcard file type filter
            picker.FileTypeFilter.Add("*");


            // Associate the picker with the current window
            InitializeWithWindow.Initialize(picker, DashBoard.HWND);

            // Show the folder picker
            StorageFolder folder = await picker.PickSingleFolderAsync();
            return folder;
        }

    }
}