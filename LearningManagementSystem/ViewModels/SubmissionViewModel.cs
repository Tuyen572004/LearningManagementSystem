using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
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

        public ICommand GradeCommand { get; }

        public SubmissionViewModel()
        {
            _dao = new SqlDao();
            Submission = new Submission();
            Student = new Student();
            Assignment = new Assignment();
            User = UserService.GetCurrentUser().Result;
            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new AsyncRelayCommand(DownloadSubmission);
            DeleteCommand = new AsyncRelayCommand(DeleteSubmission, CanDeleteSubmission);
        }

        public SubmissionViewModel(Submission submission, Assignment assignment)
        {
            _dao = new SqlDao();
            Submission = submission;
            Student = _dao.GetStudentByUserId(submission.UserId);
            Assignment = assignment;
            User = UserService.GetCurrentUser().Result;
            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new AsyncRelayCommand(DownloadSubmission);
            DeleteCommand = new AsyncRelayCommand(DeleteSubmission, CanDeleteSubmission);
            GradeCommand = new RelayCommand(GradeSubmission, CanGradeSubmission);
        }

        private bool CanGradeSubmission()
        {
            return User.Role == RoleEnum.GetStringValue(Role.Teacher);
        }

        private void GradeSubmission()
        {
            _dao.UpdateSubmission(Submission);
        }

        public async Task UpdateSubmission()
        {
            try
            {
                StorageFile selectedFile = await FileHelper.ChooseFile();
                if (selectedFile == null) return; // User canceled file selection

                UpdateBusyStatus(true);

                String oldFile = Submission.FilePath;

                // Update submission details
                await UpdateSubmissionDetails(selectedFile);
                // Update the submission in the database
                _dao.UpdateSubmission(Submission);

                // delete old file
                await _cloudinaryService.DeleteFileByUriAsync(oldFile);

                UpdateBusyStatus(false);

                // TEST RAISE AFTER UPDATE BUSY TO FIX NOTIFICATION OF COLLECTION
                //RaisePropertyChanged(nameof(Submission));
            }
            catch (Exception ex)
            {
                UpdateBusyStatus(false);
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error updating submission: {ex.Message}\n Please try again later."));
            }
        }

        public bool CanUpdateSubmission()
        {
            return User.Role == "student" && DateTime.Now < Assignment.DueDate;
        }

        public async Task UpdateSubmissionDetails(StorageFile selectedFile)
        {
            Submission.SubmissionDate = DateTime.Now;
            Submission.FileName = selectedFile.Name;
            Submission.FileType = selectedFile.FileType;
            Submission.FilePath = await _cloudinaryService.UploadFileAsync(selectedFile.Path);
        }

        public async Task DownloadSubmission()
        {
            try
            {
                StorageFolder selectedFolder = await FileHelper.ChooseFolder();

                if (selectedFolder == null) return; // User canceled folder selection

                UpdateBusyStatus(true);

                // build path to file
                var targetFilePath = selectedFolder.Path;
                targetFilePath = Path.Combine(targetFilePath, Submission.FileName);

                await _cloudinaryService.DownloadFileAsync(Submission.FilePath, targetFilePath);

                UpdateBusyStatus(false);

                // Send success message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Successful", $"File downloaded successfully to {selectedFolder.Path}"));
            }
            catch (Exception ex)
            {
                UpdateBusyStatus(false);
                // Send error message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message} \n Please try again later."));
            }
        }

        public async Task DeleteSubmission()
        {
            try
            {
                UpdateBusyStatus(true);
                if (Submission.FilePath != null)
                {
                    await _cloudinaryService.DeleteFileByUriAsync(Submission.FilePath);
                }
                _dao.DeleteSubmissionById(Submission.Id);
                UpdateBusyStatus(false);
                WeakReferenceMessenger.Default.Send(new DeleteSubmissionMessage(this));
                
            }
            catch (Exception ex)
            {
                UpdateBusyStatus(false);
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error deleting submission: {ex.Message}\n Please try again later."));
            }
        }

        public bool CanDeleteSubmission()
        {
            var role = UserService.GetCurrentUser().Result.Role.ToString();
            return role == "student" && DateTime.Now < Assignment.DueDate;
        }

        private void UpdateBusyStatus(bool isBusy)
        {
            WeakReferenceMessenger.Default.Send(new BusyMessage(isBusy));
        }
    }
}