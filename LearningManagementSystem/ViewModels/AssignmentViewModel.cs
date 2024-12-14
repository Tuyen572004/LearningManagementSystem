using CloudinaryDotNet.Core;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace LearningManagementSystem.ViewModels
{
    public class AssignmentViewModel : BaseViewModel
    {
        private Assignment _assignment;

        //    Reason why we have to set manually eventhough we have Fody.PropertyChanged
        //    Hanlde nested property change.If we just do as normally {get;set;} it will only trigger when the whole object is changed(not the attributes)
        // Error : Due date change but Due Status is not updated even though I use PropertyChanged += UpdateAssignmentDependentProperties, DependsOn, whatever I tried
        public Assignment Assignment
        {
            get => _assignment;
            set
            {
                if (_assignment != value)
                {
                    if (_assignment != null) // Unsubscribe from previous assignment
                    {
                        _assignment.PropertyChanged -= UpdateAssignmentDependentProperties;
                    }

                    _assignment = value;

                    if (_assignment != null)
                    {
                        _assignment.PropertyChanged += UpdateAssignmentDependentProperties;
                    }
                }
            }
        }

        public DateTimeOffset DueDate
        {
            get => new DateTimeOffset(Assignment.DueDate);
            set
            {
                if (Assignment.DueDate.Date != value.Date)
                {
                    Assignment.DueDate = new DateTime(value.Year, value.Month, value.Day, Assignment.DueDate.Hour, Assignment.DueDate.Minute, Assignment.DueDate.Second);
                    RaisePropertyChanged(nameof(DueDate));
                    RaisePropertyChanged(nameof(DueTime));
                }
            }
        }

        public TimeSpan DueTime
        {
            get => Assignment.DueDate.TimeOfDay;
            set
            {
                if (Assignment.DueDate.TimeOfDay != value)
                {
                    Assignment.DueDate = new DateTime(Assignment.DueDate.Year, Assignment.DueDate.Month, Assignment.DueDate.Day, value.Hours, value.Minutes, value.Seconds);
                    RaisePropertyChanged(nameof(DueDate));
                    RaisePropertyChanged(nameof(DueTime));
                }
            }
        }


        private void UpdateAssignmentDependentProperties(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Assignment));
            if (e.PropertyName == nameof(Assignment.DueDate))
            {
                RaisePropertyChanged(nameof(DueStatus));
            }

        }

        public FullObservableCollection<SubmissionViewModel> Submissions { get; set; }

        public ICommand ExportToExcelCommand { get; }

        public bool IsBusy { get; set; }


        private async Task ExportToExcel()
        {
            try
            {
                var folder = await FileHelper.ChooseFolder();
                if (folder == null) return;

                IsBusy = true;

                string filename = $"{Assignment.Title}_Submissions.xlsx";
                string filePath = System.IO.Path.Combine(folder.Path, filename);

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Submissions");

                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Id");
                headerRow.CreateCell(1).SetCellValue("Student Name");
                headerRow.CreateCell(2).SetCellValue("Submission Date");
                headerRow.CreateCell(3).SetCellValue("File Name");
                headerRow.CreateCell(4).SetCellValue("Grade");

                for (int i = 0; i < Submissions.Count; i++)
                {
                    var submission = Submissions[i];
                    IRow row = sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(submission.Student.StudentCode);
                    row.CreateCell(1).SetCellValue(submission.Student.StudentName);
                    row.CreateCell(2).SetCellValue(submission.Submission.SubmissionDate.ToString("yyyy-MM-dd"));
                    row.CreateCell(3).SetCellValue(submission.Submission.FileName);
                    if (submission.Submission.Grade != null)
                    {
                        row.CreateCell(4).SetCellValue(submission.Submission.Grade.Value);
                    }
                    else
                    {
                        row.CreateCell(4).SetCellValue("Not Graded");
                    }
                }

                for (int col = 0; col < 5; col++)
                {
                    sheet.AutoSizeColumn(col);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                workbook.Close();
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Export Successful", $"Submissions exported to {filePath}"));
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error exporting to Excel: {ex.Message}\n Please try again later."));
            }
        }



        public User User { get; set; }
        private readonly CloudinaryService _cloudinaryService = new CloudinaryService();

        public bool IsTeacher { get; set; }
        public bool IsStudent => !IsTeacher;

        public bool IsEditing { get; set; }

        public bool IsNotEditing => !IsEditing;

        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Assignment";

        private bool IsDue => Assignment.DueDate < DateTime.Now;

        public string DueStatus => IsDue ? "Closed" : "On Going";

        public bool SubmitVisibility => Submissions.Count == 0 && !IsDue;

        public ICommand SubmitCommand { get; }
        public ICommand ToggleEditCommand { get; }
        public ICommand ChangeAttachmentCommand { get; }

        public ICommand DownloadAttachmentCommand { get; }

        public ICommand DeleteAttachmentCommand { get; }

        public ICommand AddAssignmentCommand { get; }



        private readonly IDao _dao = new SqlDao();
        private readonly UserService _userService;
        public FileHelper FileHelper = new FileHelper();

        // Events for dependencies
        public event Action EditingChanged;


        public AssignmentViewModel()
        {
            _userService = new UserService();
            checkRole();

            Submissions = new FullObservableCollection<SubmissionViewModel>();

            SubmitCommand = new AsyncRelayCommand(SubmitAssignment, canSubmitAssignment);

            ToggleEditCommand = new RelayCommand(ToggleEditMode);

            ChangeAttachmentCommand = new AsyncRelayCommand(ChangeAttachment);

            DownloadAttachmentCommand = new AsyncRelayCommand(DownloadAttachment);

            DeleteAttachmentCommand = new AsyncRelayCommand(DeleteAttachment, CanDeleteAttachment);

            AddAssignmentCommand = new RelayCommand(AddAssignment, CanAddAssignment);

            // Register to receive delete messages
            WeakReferenceMessenger.Default.Register<DeleteSubmissionMessage>(this, async (r, m) =>
            {
                DeleteSubmission(m.Value as SubmissionViewModel);

            });

            Assignment = new Assignment();

            // Subscribe to events
            EditingChanged += UpdateEditingDependentProperties;

            Submissions.CollectionChanged += OnSubmissionsChanged;

            ExportToExcelCommand = new AsyncRelayCommand(ExportToExcel);
        }

        private bool CanAddAssignment()
        {
            return IsTeacher;
        }

        private void AddAssignment()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Assignment.Title))
                {
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Assignment title cannot be empty"));
                    return;
                }
                IsBusy = true;
                _dao.AddAssignment(Assignment);
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Success", "Assignment added successfully"));
            }
            catch (Exception e)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error adding assignment: {e.Message}\n Please try again later."));
            }

        }

        private bool CanDeleteAttachment()
        {
            return Assignment.FilePath != null && IsTeacher;
        }

        private async Task DeleteAttachment()
        {
            try
            {
                IsBusy = true;
                await _cloudinaryService.DeleteFileByUriAsync(Assignment.FilePath);

                _dao.DeleteAttachmentByAssignmentId(Assignment.Id); ;

                Assignment.FileName = null;
                Assignment.FilePath = null;
                Assignment.FileType = null;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error deleting attachment: {ex.Message}\n Please try again later."));
            }
        }

        private async Task DownloadAttachment()
        {
            if (string.IsNullOrEmpty(Assignment.FileName))
            {
                WeakReferenceMessenger.Default.Send(new DialogMessage("No attachment", "No file attached to download"));
                return;
            }
            try
            {
                StorageFolder selectedFolder = await FileHelper.ChooseFolder();

                if (selectedFolder == null) return; // User canceled folder selection

                IsBusy = true;
                var targetFilePath = selectedFolder.Path;
                targetFilePath = Path.Combine(targetFilePath, Assignment.FileName);

                await _cloudinaryService.DownloadFileAsync(Assignment.FilePath, targetFilePath);
                IsBusy = false;

                // Send success message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Successful", $"File downloaded successfully to {selectedFolder.Path}"));

            }
            catch (Exception ex)
            {
                IsBusy = false;
                // Send error message
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message}\n Please try again later."));
            }
        }

        private bool canSubmitAssignment()
        {
            return SubmitVisibility;
        }

        public async void checkRole()
        {
            User = await UserService.GetCurrentUser();
            var role = User.Role;
            IsTeacher = role == "teacher";
        }

        public void LoadSubmissions()
        {
            if (IsTeacher)
            {
                List<Submission> submissions = _dao.GetSubmissionsByAssignmentId(Assignment.Id);
                foreach (var submission in submissions)
                {
                    Submissions.Add(new SubmissionViewModel(submission, Assignment));
                }
            }
            else
            {
                var submission = _dao.GetSubmissionsByAssignmentIdAndUserId(Assignment.Id, User.Id);
                var student = _dao.GetStudentByUserId(User.Id);
                for (int i = 0; i < submission.Count; i++)
                {
                    Submissions.Add(new SubmissionViewModel(submission[i], Assignment));
                }
            }
        }



        public async Task SubmitAssignment()
        {
            StorageFile file = await FileHelper.ChooseFile();

            if (file != null)
            {
                try
                {
                    IsBusy = true;
                    string filePath = await _cloudinaryService.UploadFileAsync(file.Path);
                    string fileName = file.Name;
                    string fileType = file.FileType;

                    var submission = new Submission
                    {
                        AssignmentId = Assignment.Id,
                        UserId = User.Id,
                        SubmissionDate = DateTime.Now,
                        FilePath = filePath,
                        FileName = fileName,
                        FileType = fileType
                    };

                    // Save submission to your database here
                    _dao.SaveSubmission(submission);
                    Submissions.Add(new SubmissionViewModel(submission, Assignment));
                    IsBusy = false;
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error submitting assignment: {ex.Message}\n Please try again later."));
                }

            }
        }

        public void DeleteSubmission(SubmissionViewModel model)
        {
            Submissions.Remove(model);
        }

        private void ToggleEditMode()
        {
            if (IsEditing)
            {
                if (string.IsNullOrWhiteSpace(Assignment.Title))
                {
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", "Assignment title cannot be empty"));
                    return;
                }
                // Save changes
                _dao.UpdateAssignment(Assignment);

            }
            IsEditing = !IsEditing;
        }

        private async Task ChangeAttachment()
        {
            StorageFile selectedFile = await FileHelper.ChooseFile();
            if (selectedFile == null) return; // User canceled file selection

            try
            {
                IsBusy = true;
                // Save the new file
                Assignment.FilePath = await _cloudinaryService.UploadFileAsync(selectedFile.Path);

                // Update assignment attachment details
                Assignment.FileName = selectedFile.Name;
                Assignment.FileType = selectedFile.FileType;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error changing attachment: {ex.Message}\n Please try again later."));
            }

        }


        private void OnSubmissionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Submissions));
            RaisePropertyChanged(nameof(SubmitVisibility));
            RaisePropertyChanged(nameof(SubmitCommand));
        }

        private void UpdateEditingDependentProperties()
        {
            RaisePropertyChanged(nameof(IsEditing));
            RaisePropertyChanged(nameof(IsNotEditing));
            RaisePropertyChanged(nameof(EditButtonText));
        }




    }
}

