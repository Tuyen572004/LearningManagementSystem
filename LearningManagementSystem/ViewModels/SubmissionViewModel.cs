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

        private CloudinaryService _cloudinaryService = new CloudinaryService();


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

            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new RelayCommand(DownloadSubmission);
            DeleteCommand = new RelayCommand(DeleteSubmission);
        }

        public SubmissionViewModel(Submission submission, Assignment assignment)
        {
            _dao = new SqlDao();
            Submission = submission;
            Student = _dao.GetStudentByUserId(submission.UserId);
            Assignment = assignment;

            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new RelayCommand(DownloadSubmission);
            DeleteCommand = new RelayCommand(DeleteSubmission);


        }



        public async Task UpdateSubmission()
        {
            try
            {

                StorageFile selectedFile = await FileHelper.ChooseFile();
                if (selectedFile == null) return; // User canceled file selection


                // Update submission details
                await UpdateSubmissionDetails(selectedFile);

                // delete old file
                await _cloudinaryService.DeleteFileByUriAsync(Submission.FilePath);

                RaisePropertyChanged(nameof(Submission));

                // Update the submission in the database
                _dao.UpdateSubmission(Submission);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating assignment: {ex.Message}");
            }
        }

        public async Task<bool> CanUpdateSubmission()
        {
            User user = await userService.GetCurrentUser();
            return user.Role == "Student" && DateTime.Now < Assignment.DueDate;
        }

        public async Task UpdateSubmissionDetails(StorageFile selectedFile)
        {
            Submission.SubmissionDate = DateTime.Now;
            Submission.FileName = selectedFile.Name;
            Submission.FileType = selectedFile.FileType;
            Submission.FilePath =await _cloudinaryService.UploadFileAsync(selectedFile.Path);
        }

        public async void DownloadSubmission()
        {
            try
            {
                StorageFolder selectedFolder = await FileHelper.ChooseFolder();

                if(selectedFolder == null) return; // User canceled folder selection

                // build path to file
                var targetFilePath = selectedFolder.Path;
                targetFilePath = Path.Combine(targetFilePath, Submission.FileName);

                await _cloudinaryService.DownloadFileAsync(Submission.FilePath, targetFilePath);

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
            WeakReferenceMessenger.Default.Send(new DeleteMessage(Submission));
        }

        public async Task<bool> CanDeleteSubmission(SubmissionViewModel submissionViewModel)
        {
            var role = await userService.getCurrentUserRole();
            return role == "Student" && DateTime.Now < submissionViewModel.Assignment.DueDate;
        }
    }
}
