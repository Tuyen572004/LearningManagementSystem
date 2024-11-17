using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;


namespace LearningManagementSystem.ViewModels
{
    public class SubmissionViewModel : BaseViewModel
    {
        public Submission Submission { get; set; }
        public Student Student { get; set; }

        public User User { get; set; }

        public Assignment Assignment { get; set; }


        private readonly IDao _dao;
        private readonly FileHelper FileHelper = new FileHelper();
        private readonly UserService userService = new UserService();


        public ICommand UpdateCommand { get; }
        public ICommand DownloadCommand { get; }

        public ICommand DeleteCommand { get; }

        public SubmissionViewModel()
        {
            _dao = new SqlDao();
            Submission = new Submission();
            Student = new Student();
            Assignment = new Assignment();

            UpdateCommand = new RelayCommand(UpdateAssignment);
            DownloadCommand = new RelayCommand(DownloadSubmission);
            DeleteCommand = new RelayCommand(DeleteSubmission);
        }

        public SubmissionViewModel(Submission submission, Assignment assignment)
        {
            _dao = new SqlDao();
            Submission = submission;
            Student = _dao.GetStudentByUserId(submission.UserId);
            Assignment = assignment;

            UpdateCommand = new RelayCommand(UpdateAssignment);
            DownloadCommand = new RelayCommand(DownloadSubmission);
            DeleteCommand = new RelayCommand(DeleteSubmission);


        }



        public async void UpdateAssignment()
        {
            try
            {

                StorageFile selectedFile = await FileHelper.ChooseFile();
                if (selectedFile == null) return; // User canceled file selection

                // Save the new file
                await FileHelper.SaveFile(selectedFile);

                // Update submission details
                UpdateSubmissionDetails(selectedFile);

                RaisePropertyChanged(nameof(Submission));

                // Update the submission in the database
                _dao.UpdateSubmission(Submission);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating assignment: {ex.Message}");
            }
        }

        public string GetOldFilePath(string oldFileName)
        {
            return !string.IsNullOrWhiteSpace(oldFileName) ? Path.Combine("D:\\Files\\Submissions", oldFileName) : null;
        }

        public void UpdateSubmissionDetails(StorageFile selectedFile)
        {
            Submission.SubmissionDate = DateTime.Now;
            Submission.FileName = selectedFile.Name;
            Submission.FileType = selectedFile.FileType;
            Submission.FilePath = Path.Combine("D:\\Files\\Submissions", selectedFile.Name);
        }

        public void DeleteOldFile(string oldFilePath)
        {
            if (!string.IsNullOrWhiteSpace(oldFilePath) && File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }


        private async Task<bool> CanUpdateAssignment(SubmissionViewModel submissionViewModel)
        {
            var role = await userService.getCurrentUserRole();
            return role == "Student";
        }

        public async void DownloadSubmission()
        {
            try
            {
                // Ensure the file exists in the database folder
                string sourceFilePath = Path.Combine("D:\\Files\\Submissions", Submission.FileName);
                if (!File.Exists(sourceFilePath))
                {
                    throw new FileNotFoundException("The file does not exist in the database folder.");
                }

                // Copy the file to the C:\\Downloads folder
                StorageFolder selectedFolder = await FileHelper.ChooseFolder();

                if(selectedFolder == null) return; // User canceled folder selection

                var targetFilePath = Path.Combine(selectedFolder.Path, Submission.FileName);

                File.Copy(sourceFilePath, targetFilePath, true);

                // Send success message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Successful", $"File downloaded successfully to {selectedFolder.Path}"));

            }
            catch (Exception ex)
            {
                // Send error message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message}"));
            }
        }

        public void DeleteSubmission()
        {
            // Send delete message
            WeakReferenceMessenger.Default.Send(new DeleteSubmissionMessage(this));
        }
    }
}
