using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace LearningManagementSystem.ViewModels
{
    public class AssignmentViewModel : BaseViewModel
    {
        public Assignment Assignment { get; set; }

        public string DueStatus => IsDue ? "Closed" : "On Going";

        public FullObservableCollection<SubmissionViewModel> Submissions { get; set; }


        public string DueDate
        {
            get
            {
                return Assignment.DueDate.ToString("dd/MM/yyyy, HH:mm");
            }
            set
            {
                Assignment.DueDate = DateTime.Parse(value);
            }
        }

        public bool HasNoSubmission => Submissions.Count == 0;

        public bool IsDue
        {
            get
            {
                return Assignment.DueDate < DateTime.Now;
            }
        }

        private readonly UserService _userService;
        public bool IsTeacher { get; set; }
        public bool IsStudent { get; set; }

        private readonly IDao _dao = new SqlDao();

        public ICommand SubmitCommand { get; }

        

        public User User { get; set; }



        public AssignmentViewModel()
        {
            _userService = new UserService();



            // Initialize submissions collection
            Submissions = new FullObservableCollection<SubmissionViewModel>();

            Submissions.CollectionChanged += Submissions_CollectionChanged;

            SubmitCommand = new RelayCommand(SubmitAssignment);

            checkRole();

            // Register to receive delete messages
            WeakReferenceMessenger.Default.Register<DeleteSubmissionMessage>(this, (r, m) =>
            {
                DeleteSubmission(m.Value);
            });


        }

        private void Submissions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasNoSubmission));
        }
        public async void checkRole()
        {
            User = await _userService.GetCurrentUser();
            var role = User.Role;
            IsTeacher = role == "Teacher";
            IsStudent = !IsTeacher;
        }

        public void LoadSubmissions()
        {
            if (IsTeacher)
            {
                List<Submission> submissions = _dao.GetSubmissionsByAssignmentId(Assignment.Id);
                foreach (var submission in submissions)
                {
                    Submissions.Add(new SubmissionViewModel(submission,Assignment));
                }
            }
            else
            {
                var submission = _dao.GetSubmissionsByAssignmentIdAndUserId(Assignment.Id, User.Id);
                var student = _dao.GetStudentByUserId(User.Id);
                for (int i = 0; i < submission.Count; i++)
                {
                    Submissions.Add(new SubmissionViewModel(submission[i],Assignment));
                }

            }
        }

        public FileHelper FileHelper = new FileHelper();

        public async void SubmitAssignment()
        {

            StorageFile file = await FileHelper.ChooseFile();

            if (file != null)
            {
                await FileHelper.SaveFile(file);
                var pathFolder = "D:\\Files\\Submissions";
                string filePath = $"{pathFolder}\\{file.Name}"; // This is the path that will be saved in the database
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
            }
            else
            {
                // Handle the case where no file was selected
                return;
            }
        }

        public void DeleteSubmission(SubmissionViewModel model)
        {
            try
            {
                model.DeleteOldFile(model.GetOldFilePath(model.Submission.FileName));

                _dao.DeleteSubmissionById(model.Submission.Id);

                Submissions.Remove(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting submission: {ex.Message}");
            }
        }

    }
}
