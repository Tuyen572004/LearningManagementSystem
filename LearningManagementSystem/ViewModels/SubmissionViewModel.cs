﻿using CloudinaryDotNet;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.CloudinaryService;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for handling submission-related operations.
    /// </summary>
    public class SubmissionViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the submission details.
        /// </summary>
        public Submission Submission { get; set; }

        /// <summary>
        /// Gets or sets the student details.
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// Gets or sets the user details.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the assignment details.
        /// </summary>
        public Assignment Assignment { get; set; }

        private ICloudinaryService _cloudinaryService = App.Current.Services.GetService<ICloudinaryService>();

        private readonly IDao _dao;
        private readonly FileHelper FileHelper = new FileHelper();
        private readonly UserService userService = new UserService();

        /// <summary>
        /// Command to update the submission.
        /// </summary>
        public ICommand UpdateCommand { get; }

        /// <summary>
        /// Command to download the submission.
        /// </summary>
        public ICommand DownloadCommand { get; }

        /// <summary>
        /// Command to delete the submission.
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Command to grade the submission.
        /// </summary>
        public ICommand GradeCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionViewModel"/> class.
        /// </summary>
        public SubmissionViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>(); ;
            Submission = new Submission();
            Student = new Student();
            Assignment = new Assignment();
            User = UserService.GetCurrentUser().Result;
            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new AsyncRelayCommand(DownloadSubmission);
            DeleteCommand = new AsyncRelayCommand(DeleteSubmission, CanDeleteSubmission);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionViewModel"/> class with specified submission and assignment.
        /// </summary>
        /// <param name="submission">The submission details.</param>
        /// <param name="assignment">The assignment details.</param>
        public SubmissionViewModel(Submission submission, Assignment assignment)
        {
            _dao = App.Current.Services.GetService<IDao>(); ;
            Submission = submission;
            Student = _dao.GetStudentByUserId(submission.UserId);
            Assignment = assignment;
            User = UserService.GetCurrentUser().Result;
            UpdateCommand = new AsyncRelayCommand(UpdateSubmission);
            DownloadCommand = new AsyncRelayCommand(DownloadSubmission);
            DeleteCommand = new AsyncRelayCommand(DeleteSubmission, CanDeleteSubmission);
            GradeCommand = new RelayCommand(GradeSubmission, CanGradeSubmission);
        }

        /// <summary>
        /// Determines whether the submission can be graded.
        /// </summary>
        /// <returns><c>true</c> if the submission can be graded; otherwise, <c>false</c>.</returns>
        private bool CanGradeSubmission()
        {
            return User.Role == RoleEnum.GetStringValue(Role.Teacher);
        }

        /// <summary>
        /// Grades the submission.
        /// </summary>
        private void GradeSubmission()
        {
            _dao.UpdateSubmission(Submission);
        }

        /// <summary>
        /// Updates the submission.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error updating submission: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Determines whether the submission can be updated.
        /// </summary>
        /// <returns><c>true</c> if the submission can be updated; otherwise, <c>false</c>.</returns>
        public bool CanUpdateSubmission()
        {
            return User.Role == "student" && DateTime.Now < Assignment.DueDate;
        }

        /// <summary>
        /// Updates the submission details.
        /// </summary>
        /// <param name="selectedFile">The selected file.</param>
        public async Task UpdateSubmissionDetails(StorageFile selectedFile)
        {
            Submission.SubmissionDate = DateTime.Now;
            Submission.FileName = selectedFile.Name;
            Submission.FileType = selectedFile.FileType;
            Submission.FilePath = await _cloudinaryService.UploadFileAsync(selectedFile.Path);
        }

        /// <summary>
        /// Downloads the submission.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message} \nPlease try again later."));
            }
        }

        /// <summary>
        /// Deletes the submission.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error deleting submission: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Determines whether the submission can be deleted.
        /// </summary>
        /// <returns><c>true</c> if the submission can be deleted; otherwise, <c>false</c>.</returns>
        public bool CanDeleteSubmission()
        {
            var role = UserService.GetCurrentUser().Result.Role.ToString();
            return role == "student" && DateTime.Now < Assignment.DueDate;
        }

        /// <summary>
        /// Updates the busy status.
        /// </summary>
        /// <param name="isBusy">if set to <c>true</c> [is busy].</param>
        private void UpdateBusyStatus(bool isBusy)
        {
            WeakReferenceMessenger.Default.Send(new BusyMessage(isBusy));
        }
    }
}