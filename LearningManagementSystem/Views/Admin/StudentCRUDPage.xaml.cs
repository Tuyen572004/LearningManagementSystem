#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using LearningManagementSystem.ViewModels;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using LearningManagementSystem.Controls;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StudentCRUDPage : Page, IDisposable
    {
        private readonly StudentReaderViewModel _readerViewModel;
        private readonly StudentCUDViewModel _cudViewModel;
        private bool disposedValue;

        public StudentCRUDPage()
        {
            this.InitializeComponent();

            _readerViewModel = new StudentReaderViewModel(App.Current.Services.GetService<IDao>()!)
            {
                RowsPerPage = 5
            };
            _readerViewModel.GetStudents();

            _cudViewModel = new StudentCUDViewModel(App.Current.Services.GetService<IDao>()!)
            {
                RowsPerPage = 5
            };
            _cudViewModel.RefreshManagingStudents();

            StudentReaderDisplayer.TableView.ItemDoubleTapped += _cudViewModel.StudentTransferHandler;
            StudentCUDDisplayer.TableView.IsEditable = true;

            OnSelectedRemoving += _cudViewModel.StudentsRemoveHandler;
            AllSelectedStudentsTransferred += _cudViewModel.StudentsTransferHandler;
            StudentsCreationInitiated += _cudViewModel.StudentsCreationHandler;
            StudentsUpdateInitiated += _cudViewModel.StudentsUpdateHandler;
            StudentsDeletionInitiated += _cudViewModel.StudentsDeleteHandler;
            OnStudentsErrorsAdded += _cudViewModel.StudentsErrorsAddedHandler;

            _cudViewModel.StudentsSelectionChanged += StudentCUDDisplayer.TableView.ItemsReselectionHandler;
            _cudViewModel.OnStudentsUpdated += OnStudentsUpdatedHandler;
            _cudViewModel.OnInvalidStudentsTranferred += InvalidStudentsTransferredHandler;

        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StudentReaderDisplayer.TableView.ItemDoubleTapped -= _cudViewModel.StudentTransferHandler;

                    OnSelectedRemoving -= _cudViewModel.StudentsRemoveHandler;
                    AllSelectedStudentsTransferred -= _cudViewModel.StudentsTransferHandler;
                    StudentsCreationInitiated -= _cudViewModel.StudentsCreationHandler;
                    StudentsUpdateInitiated -= _cudViewModel.StudentsUpdateHandler;
                    StudentsDeletionInitiated -= _cudViewModel.StudentsDeleteHandler;
                    OnStudentsErrorsAdded -= _cudViewModel.StudentsErrorsAddedHandler;

                    _cudViewModel.StudentsSelectionChanged -= StudentCUDDisplayer.TableView.ItemsReselectionHandler;
                    _cudViewModel.OnStudentsUpdated -= OnStudentsUpdatedHandler;
                    _cudViewModel.OnInvalidStudentsTranferred -= InvalidStudentsTransferredHandler;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StudentCRUDPage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private IList<object> GetSelectedItemsInCUD()
        {
            return StudentCUDDisplayer.TableView.GetSelectedItems();
        }
        private void ShowCUDInfoBar(InfoBarMessage message)
        {
            StudentCUDDisplayer.TableView.ShowInfoBar(message);
        }

        // ---

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool _isAssigningUser = false;
        private void AssignUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAssigningUser)
            {
                return;
            }
            _isAssigningUser = true;

            var selectedItems = GetSelectedItemsInCUD();
            if (selectedItems.Count != 0)
            {
                Frame.Navigate(typeof(UserAssignmentForStudentsPage), selectedItems);
            }
            else
            {
                ShowCUDInfoBar(new()
                {
                    Title = "User Assignment Warning",
                    Message = "Please select the student(s) to be assigned!",
                    Severity = InfoBarMessageSeverity.Warning
                });
            }
            _isAssigningUser = false;
        }

        public event EventHandler? StudentsCreationInitiated;
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            StudentsCreationInitiated?.Invoke(this, new EventArgs());
        }
        public event EventHandler<IList<object>>? StudentsUpdateInitiated;
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = StudentCUDDisplayer.TableView.GetSelectedItems();
            if (selectedItems.Count != 0)
            {
                StudentsUpdateInitiated?.Invoke(this, selectedItems);
            }
        }
        public event EventHandler<IList<object>>? StudentsDeletionInitiated;
        public void HandleStudentsUpdated(object? sender, (
            IList<StudentVer2> updatedStudents,
            IList<(StudentVer2 student, IEnumerable<string> errors)> invalidStudentsInfo
            ) e)
        {
            if (sender is null)
            {
                return;
            }
            if (e.updatedStudents.Count > 0)
            {
                OnSelectedRemoving?.Invoke(this, e.updatedStudents.Cast<object>().ToList());
            }
            if (e.invalidStudentsInfo.Count > 0)
            {
                OnStudentsErrorsAdded?.Invoke(this, e.invalidStudentsInfo);
                
            }
            StudentCUDDisplayer.TableView.ItemsReselectionHandler?.Invoke(
                this,
                e.invalidStudentsInfo.Select(info => info.student).Cast<object>().ToList()
                );

            var displayTitle = "Student(s) Updated";
            if (e.updatedStudents.Count == 0)
            {
                displayTitle += "?!";
            }
            if (e.invalidStudentsInfo.Count > 0)
            {
                displayTitle += " with failure(s)";
            }

            var displayMessage = e.invalidStudentsInfo.Count > 0
                ? $"{e.updatedStudents.Count} students have been modified successfully. \n{e.invalidStudentsInfo.Count} students have errors."
                : $"{e.updatedStudents.Count} students have been modified successfully.";
            StudentCUDDisplayer.TableView.ShowInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Info
            });

            StudentReaderDisplayer.TableView.RefreshData();
        }
        public event EventHandler<IList<(StudentVer2 student, IEnumerable<String> errors)>>? OnStudentsErrorsAdded; 
        public EventHandler<(
            IList<StudentVer2> updatedStudents,
            IList<(StudentVer2 student, IEnumerable<string> errors)> invalidStudentsInfo
            )> OnStudentsUpdatedHandler => HandleStudentsUpdated;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = StudentCUDDisplayer.TableView.GetSelectedItems();
            if (selectedItems.Count != 0)
            {
                StudentsDeletionInitiated?.Invoke(this, selectedItems);
            }
        }

        public event EventHandler<IList<object>>? AllSelectedStudentsTransferred;

        public void HandleInvalidStudentsTransfered(object? sender, IList<StudentVer2> invalidStudents)
        {
            if (sender is null)
            {
                return;
            }
            var invalidIds = invalidStudents.Select(student => student.Id).ToList();
            var displayTitle = "Student Transfering Warning";
            var displayMessage = $"""
                Ignoring {invalidStudents.Count} student(s) with the following Id(s): {String.Join(", ", invalidIds)}.
                They have already existed in the transferred table.
                """;
            StudentReaderDisplayer.TableView.ShowInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Warning
            });
        }
        public EventHandler<IList<StudentVer2>> InvalidStudentsTransferredHandler => HandleInvalidStudentsTransfered;
        private void EditAllButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = StudentReaderDisplayer.TableView.GetSelectedItems();
            if (selectedItems.Count != 0)
            {
                AllSelectedStudentsTransferred?.Invoke(this, selectedItems);
            }
        }

        public event EventHandler<IList<object>>? OnSelectedRemoving;
        private void RemoveAllButton_Click(object _, RoutedEventArgs __)
        {
            var selectedItems = StudentCUDDisplayer.TableView.GetSelectedItems();
            if (selectedItems.Count != 0)
            {
                OnSelectedRemoving?.Invoke(this, selectedItems);
            }
        }
        private void HandleStudentsChanged(object? sender, IList<object> selectedItems)
        {
            if (sender is null)
            {
                return;
            }
            if (selectedItems.Count != 0)
            {
                OnSelectedRemoving?.Invoke(this, selectedItems);
            }
        }
        public EventHandler<IList<object>> StudentsChangedHandler => HandleStudentsChanged;
    }
}
