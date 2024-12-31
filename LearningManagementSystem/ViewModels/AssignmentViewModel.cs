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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing assignments.
    /// </summary>
    public class AssignmentViewModel : BaseViewModel
    {
        private Assignment _assignment;

        /// <summary>
        /// Gets or sets the assignment.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the due date of the assignment.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the due time of the assignment.
        /// </summary>
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

        /// <summary>
        /// Updates dependent properties when the assignment properties change.
        /// </summary>
        private void UpdateAssignmentDependentProperties(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Assignment));
            if (e.PropertyName == nameof(Assignment.DueDate))
            {
                RaisePropertyChanged(nameof(DueStatus));
            }
        }

        /// <summary>
        /// Collection of submissions for the assignment.
        /// </summary>
        public FullObservableCollection<SubmissionViewModel> Submissions { get; set; }

        /// <summary>
        /// Command to export submissions to an Excel file.
        /// </summary>
        public ICommand ExportToExcelCommand { get; }

        /// <summary>
        /// Indicates whether the ViewModel is busy.
        /// </summary>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Exports the submissions to an Excel file.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error exporting to Excel: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// The current user.
        /// </summary>
        public User User { get; set; }
        private readonly ICloudinaryService _cloudinaryService = App.Current.Services.GetService<ICloudinaryService>();
        private readonly IDao _dao = App.Current.Services.GetService<IDao>();

        /// <summary>
        /// Indicates whether the current user is a teacher.
        /// </summary>
        public bool IsTeacher { get; set; }

        /// <summary>
        /// Indicates whether the current user is a student.
        /// </summary>
        public bool IsStudent { get; set; }

        /// <summary>
        /// Indicates whether the assignment is being edited.
        /// </summary>
        public bool IsEditing { get; set; }

        /// <summary>
        /// Indicates whether the assignment is not being edited.
        /// </summary>
        public bool IsNotEditing => !IsEditing;

        /// <summary>
        /// Text for the edit button.
        /// </summary>
        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Assignment";

        /// <summary>
        /// Indicates whether the assignment is due.
        /// </summary>
        private bool IsDue => Assignment.DueDate < DateTime.Now;

        /// <summary>
        /// Status of the assignment due date.
        /// </summary>
        public string DueStatus => IsDue ? "Closed" : "On Going";

        /// <summary>
        /// Visibility of the submit button.
        /// </summary>
        public bool SubmitVisibility => Submissions.Count == 0 && !IsDue;

        /// <summary>
        /// Command to submit the assignment.
        /// </summary>
        public ICommand SubmitCommand { get; }

        /// <summary>
        /// Command to toggle edit mode.
        /// </summary>
        public ICommand ToggleEditCommand { get; }

        /// <summary>
        /// Command to change the attachment.
        /// </summary>
        public ICommand ChangeAttachmentCommand { get; }

        /// <summary>
        /// Command to download the attachment.
        /// </summary>
        public ICommand DownloadAttachmentCommand { get; }

        /// <summary>
        /// Command to delete the attachment.
        /// </summary>
        public ICommand DeleteAttachmentCommand { get; }

        /// <summary>
        /// Command to add a new assignment.
        /// </summary>
        public ICommand AddAssignmentCommand { get; }

        private readonly UserService _userService;
        public FileHelper FileHelper = new FileHelper();

        /// <summary>
        /// Event triggered when editing state changes.
        /// </summary>
        public event Action EditingChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentViewModel"/> class.
        /// </summary>
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

        /// <summary>
        /// Determines whether a new assignment can be added.
        /// </summary>
        private bool CanAddAssignment()
        {
            return !IsStudent;
        }

        /// <summary>
        /// Adds a new assignment.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error adding assignment: {e.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Determines whether the attachment can be deleted.
        /// </summary>
        private bool CanDeleteAttachment()
        {
            return Assignment.FilePath != null && IsTeacher;
        }

        /// <summary>
        /// Deletes the attachment.
        /// </summary>
        private async Task DeleteAttachment()
        {
            try
            {
                IsBusy = true;
                await _cloudinaryService.DeleteFileByUriAsync(Assignment.FilePath);

                _dao.DeleteAttachmentByAssignmentId(Assignment.Id);

                Assignment.FileName = null;
                Assignment.FilePath = null;
                Assignment.FileType = null;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error deleting attachment: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Downloads the attachment.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Download Failed", $"Error downloading file: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Determines whether the assignment can be submitted.
        /// </summary>
        private bool canSubmitAssignment()
        {
            return SubmitVisibility;
        }

        /// <summary>
        /// Checks the role of the current user.
        /// </summary>
        public async void checkRole()
        {
            User = await UserService.GetCurrentUser();
            var role = User.Role;
            IsTeacher = role.Equals(RoleEnum.GetStringValue(Role.Teacher));
            IsStudent = role.Equals(RoleEnum.GetStringValue(Role.Student));
        }

        /// <summary>
        /// Loads the submissions for the assignment.
        /// </summary>
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

        /// <summary>
        /// Submits the assignment.
        /// </summary>
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
                    WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error submitting assignment: {ex.Message}\nPlease try again later."));
                }
            }
        }

        /// <summary>
        /// Deletes a submission.
        /// </summary>
        public void DeleteSubmission(SubmissionViewModel model)
        {
            Submissions.Remove(model);
        }

        /// <summary>
        /// Toggles the edit mode.
        /// </summary>
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

        /// <summary>
        /// Changes the attachment.
        /// </summary>
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
                WeakReferenceMessenger.Default.Send(new DialogMessage("Error", $"Error changing attachment: {ex.Message}\nPlease try again later."));
            }
        }

        /// <summary>
        /// Handles changes in the submissions collection.
        /// </summary>
        private void OnSubmissionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Submissions));
            RaisePropertyChanged(nameof(SubmitVisibility));
            RaisePropertyChanged(nameof(SubmitCommand));
        }

        /// <summary>
        /// Updates properties dependent on the editing state.
        /// </summary>
        private void UpdateEditingDependentProperties()
        {
            RaisePropertyChanged(nameof(IsEditing));
            RaisePropertyChanged(nameof(IsNotEditing));
            RaisePropertyChanged(nameof(EditButtonText));
        }
    }
}

