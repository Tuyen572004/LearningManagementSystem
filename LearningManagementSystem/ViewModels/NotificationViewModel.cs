using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services.CloudinaryService;
using LearningManagementSystem.Services.EmailService;
using LearningManagementSystem.Services.EmailService.Model;
using LearningManagementSystem.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace LearningManagementSystem.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        private Notification _notification;

        public Notification Notification
        {
            get => _notification;
            set
            {
                if (_notification != value)
                {
                    if (_notification != null)
                    {
                        _notification.PropertyChanged -= UpdateNotificationDependentProperties;
                    }

                    _notification = value;

                    if (_notification != null)
                    {
                        _notification.PropertyChanged += UpdateNotificationDependentProperties;
                    }
                }
            }
        }

        private void UpdateNotificationDependentProperties(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Notification));
        }

        public bool IsBusy { get; set; }

        public bool IsTeacher { get; set; }
        public bool IsStudent => UserService.GetCurrentUser().Result.Role.Equals(RoleEnum.GetStringValue(Role.Student));

        public bool IsEditing { get; set; }

        public bool IsNotEditing => !IsEditing;

        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Notification";

        public ICommand ToggleEditCommand { get; }
        public ICommand ChangeAttachmentCommand { get; }
        public ICommand DownloadAttachmentCommand { get; }
        public ICommand DeleteAttachmentCommand { get; }
        public ICommand AddNotificationCommand { get; }
        public ICommand SendEmailCommand { get; }

        private readonly ICloudinaryService _cloudinaryService = App.Current.Services.GetService<ICloudinaryService>();
        private readonly IDao _dao = App.Current.Services.GetService<IDao>();
        public FileHelper FileHelper = new FileHelper();
        public IEmailService emailService = App.Current.Services.GetService<IEmailService>();

        public NotificationViewModel()
        {
            ToggleEditCommand = new RelayCommand(ToggleEditMode);
            ChangeAttachmentCommand = new AsyncRelayCommand(ChangeAttachment);
            DownloadAttachmentCommand = new AsyncRelayCommand(DownloadAttachment);
            DeleteAttachmentCommand = new AsyncRelayCommand(DeleteAttachment, CanDeleteAttachment);
            AddNotificationCommand = new RelayCommand(AddNotification, CanAddNotification);
            SendEmailCommand = new AsyncRelayCommand(SendEmail);
            CheckRole();

            Notification = new Notification();
        }

        private void CheckRole()
        {
            User user = UserService.GetCurrentUser().Result;
            IsTeacher = user.Role.Equals(RoleEnum.GetStringValue(Role.Teacher));
        }

        public DateTimeOffset PostDate
        {
            get => new DateTimeOffset(Notification.PostDate);
            set
            {
                if (Notification.PostDate.Date != value.Date)
                {
                    Notification.PostDate = new DateTime(value.Year, value.Month, value.Day, Notification.PostDate.Hour, Notification.PostDate.Minute, Notification.PostDate.Second);
                    RaisePropertyChanged(nameof(PostDate));
                    RaisePropertyChanged(nameof(PostTime));
                }
            }
        }

        public TimeSpan PostTime
        {
            get => Notification.PostDate.TimeOfDay;
            set
            {
                if (Notification.PostDate.TimeOfDay != value)
                {
                    Notification.PostDate = new DateTime(Notification.PostDate.Year, Notification.PostDate.Month, Notification.PostDate.Day, value.Hours, value.Minutes, value.Seconds);
                    RaisePropertyChanged(nameof(PostDate));
                    RaisePropertyChanged(nameof(PostTime));
                }
            }
        }

        private void ToggleEditMode()
        {
            if (IsEditing)
            {
                if (string.IsNullOrWhiteSpace(Notification.Title))
                {
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Notification title cannot be empty"));
                    return;
                }
                _dao.UpdateNotification(Notification);
            }
            IsEditing = !IsEditing;
        }

        private bool CanDeleteAttachment()
        {
            return Notification.FilePath != null && IsTeacher;
        }

        private async Task DeleteAttachment()
        {
            try
            {
                IsBusy = true;
                await _cloudinaryService.DeleteFileByUriAsync(Notification.FilePath);

                _dao.DeleteAttachmentByNotificationId(Notification.Id);

                Notification.FileName = null;
                Notification.FilePath = null;
                Notification.FileType = null;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error deleting attachment: {ex.Message}\nPlease try again later."));
            }
        }

        private async Task DownloadAttachment()
        {
            if (string.IsNullOrEmpty(Notification.FileName))
            {
                WeakReferenceMessenger.Default.Send(new DialogMessage("No attachment", "No file attached to download"));
                return;
            }
            try
            {
                StorageFolder selectedFolder = await FileHelper.ChooseFolder();

                if (selectedFolder == null) return;

                IsBusy = true;
                var targetFilePath = Path.Combine(selectedFolder.Path, Notification.FileName);

                await _cloudinaryService.DownloadFileAsync(Notification.FilePath, targetFilePath);
                IsBusy = false;

                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Successful", $"File downloaded successfully to {selectedFolder.Path}"));
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message}\nPlease try again later."));
            }
        }

        private async Task ChangeAttachment()
        {
            StorageFile selectedFile = await FileHelper.ChooseFile();
            if (selectedFile == null) return;

            try
            {
                IsBusy = true;
                Notification.FilePath = await _cloudinaryService.UploadFileAsync(selectedFile.Path);

                Notification.FileName = selectedFile.Name;
                Notification.FileType = selectedFile.FileType;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error changing attachment: {ex.Message}\nPlease try again later."));
            }
        }

        private bool CanAddNotification()
        {
            return !IsStudent;
        }

        private void AddNotification()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Notification.Title))
                {
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Notification title cannot be empty"));
                    return;
                }
                IsBusy = true;
                _dao.AddNotification(Notification);
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Success", "Notification added successfully"));
            }
            catch (Exception e)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error adding notification: {e.Message}\nPlease try again later."));
            }
        }


        private EmailRequest CreateEmailRequest()
        {
            User user = UserService.GetCurrentUser().Result;
            Account sender = new Account
            {
                name = user.Username,
                email = user.Email
            };
            List<int> studentIds = _dao.findStudentIdByClassId(Notification.ClassId);
            List<StudentVer2> students = _dao.findStudentsByIdIn(studentIds);
            List<Account> to = new List<Account>();
            foreach (StudentVer2 student in students)
            {
                User studentUser = _dao.findUserById(student.UserId);
                if (studentUser != null) {
                    Account receiver = new Account
                    {
                        name = studentUser.Username,
                        email = studentUser.Email
                    };
                    to.Add(receiver);
                }
            }

            return new EmailRequest
            {
                sender = sender,
                to = to,
                subject = Notification.Title,
                htmlContent = $"<p>New notification posted by {user.Username}</p><p>{Notification.Description}</p>"
            };
            
        }

        private async Task SendEmail()
        {
            try
            {
                EmailRequest emailRequest = CreateEmailRequest();
                await emailService.sendEmail(emailRequest);
                WeakReferenceMessenger.Default.Send(new DialogMessage("Success", "Email sent successfully"));
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error sending email: {ex.Message}\nPlease try again later."));
            }
        }
    }
}
