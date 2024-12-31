#nullable enable
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI.Controls; // DataGridSortDirection
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.UI.Xaml.Data;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// ViewModel for managing Create, Update, and Delete operations for students.
    /// </summary>
    public partial class StudentCUDViewModel : BaseViewModel, ITableItemProvider, IPagingProvider, IInfoProvider, IRowStatusDeterminer, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentCUDViewModel"/> class.
        /// </summary>
        /// <param name="dao">The data access object for interacting with the database.</param>
        public StudentCUDViewModel(IDao dao)
        {
            _dao = dao;
            WeakReferenceMessenger.Default.Register<UserAssignmentMessage>(this, (r, m) =>
            {
                if (m.Target == UserAssignmentTarget.Student && m.IsConfirm)
                {
                    RefreshManagingStudents();
                }
            });
        }

        /// <summary>
        /// Releases all resources used by the <see cref="StudentCUDViewModel"/> class.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        /// <summary>
        /// The default number of rows per page.
        /// </summary>
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
        private readonly IDao _dao;

        /// <summary>
        /// Gets or sets the current page number. Should be non-zero all the time.
        /// </summary>
        public int CurrentPage { get; internal set; } = 1;
        private int _rowsPerPage = DEFAULT_ROWS_PER_PAGE;

        /// <summary>
        /// Gets or sets the number of rows per page.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is less than or equal to 0.</exception>
        [AlsoNotifyFor(nameof(PageCount))]
        public int RowsPerPage
        {
            get => _rowsPerPage;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Rows per page must be greater than 0");
                }
                _rowsPerPage = value;
            }
        }

        /// <summary>
        /// Gets the total number of items.
        /// </summary>
        [AlsoNotifyFor(nameof(PageCount))]
        public int ItemCount { get; private set; } = 0;

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int PageCount { get => ItemCount / RowsPerPage + ((ItemCount % RowsPerPage > 0) ? 1 : 0); }

        /// <summary>
        /// Gets the collection of students currently being managed.
        /// </summary>
        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];

        /// <summary>
        /// Gets the collection of items currently being managed.
        /// </summary>
        public ObservableCollection<object> ManagingItems { get; private set; } = [];

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public IEnumerable<string> IgnoringColumns => [];

        /// <summary>
        /// Gets the order of columns.
        /// </summary>
        public IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo", "EnrollmentYear", "GraduationYear"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
                ("UserId", new NegativeIntToNewMarkerConverter()),
                ("GraduationYear", new NullableIntToStringConverter()),
                ("BirthDate", new DateTimeToStringConverter()),
                ];

        /// <summary>
        /// Gets the read-only columns.
        /// </summary>
        public IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];

        /// <summary>
        /// Gets the collection of all students.
        /// </summary>
        public ObservableCollection<StudentVer2> AllStudents { get; private set; } = [];

        /// <summary>
        /// Gets or sets the sort criteria.
        /// </summary>
        public List<SortCriteria>? SortCriteria { get; private set; } = null;

        /// <summary>
        /// Navigates to the specified page number.
        /// </summary>
        /// <param name="pageNumber">The page number to navigate to.</param>
        public void NavigateToPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                return;
            }

            CurrentPage = pageNumber;
            RefreshManagingStudents();
            RaisePropertyChanged(nameof(CurrentPage));
            return;
        }

        /// <summary>
        /// Sorts the students by the specified criteria.
        /// </summary>
        private void SortByItsCriteria()
        {
            if (SortCriteria is null || SortCriteria.Count == 0)
            {
                return;
            }
            IEnumerable<StudentVer2> sortResult = AllStudents;
            foreach (var criteria in SortCriteria.AsEnumerable().Reverse())
            {
                var column = criteria.ColumnTag;
                var direction = criteria.SortDirection;
                if (direction == DataGridSortDirection.Ascending)
                {
                    sortResult = sortResult.OrderBy(s => s.GetType().GetProperty(column)?.GetValue(s));
                }
                else if (direction == DataGridSortDirection.Descending)
                {
                    sortResult = sortResult.OrderByDescending(s => s.GetType().GetProperty(column)?.GetValue(s));
                }
            }
            AllStudents = new ObservableCollection<StudentVer2>(sortResult);
        }

        /// <summary>
        /// Refreshes the collection of students currently being managed.
        /// </summary>
        public void RefreshManagingStudents()
        {
            SortByItsCriteria();
            ManagingStudents = new(
                AllStudents.AsEnumerable()
                    .Skip((CurrentPage - 1) * RowsPerPage)
                    .Take(RowsPerPage)
            );
            ManagingItems = new(ManagingStudents);
            RaisePropertyChanged(nameof(ManagingItems));
        }

        /// <summary>
        /// Refreshes the total number of items.
        /// </summary>
        public void RefreshItemCount()
        {
            ItemCount = AllStudents.Count;
            RaisePropertyChanged(nameof(ItemCount));
            if (CurrentPage > PageCount)
            {
                NavigateToPage(PageCount);
            }
        }

        /// <summary>
        /// Handles changes in the sort criteria.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The new sort criteria.</param>
        public void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            RefreshManagingStudents();
        }

        /// <summary>
        /// Gets the event handler for sort changes.
        /// </summary>
        public EventHandler<List<SortCriteria>> SortChangedHandler => HandleSortChange;

        /// <summary>
        /// Event triggered when invalid students are transferred.
        /// </summary>
        public event EventHandler<IList<StudentVer2>>? OnInvalidStudentsTranferred;

        /// <summary>
        /// Handles the transfer of a single student.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The student being transferred.</param>
        public void HandleStudentTransfer(object? sender, object e)
        {
            HandleStudentsTransfer(sender, [e]);
        }

        /// <summary>
        /// Gets the event handler for student transfers.
        /// </summary>
        public EventHandler<object> StudentTransferHandler => HandleStudentTransfer;

        /// <summary>
        /// Handles the transfer of multiple students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The list of students being transferred.</param>
        public void HandleStudentsTransfer(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            List<StudentVer2> invalidStudents = [];
            List<StudentVer2> cloningStudents = [];
            foreach (StudentVer2 student in e.Cast<StudentVer2>())
            {
                var existingStudent = AllStudents.FirstOrDefault(s => s?.Id == student.Id, null);
                if (existingStudent != null)
                {
                    invalidStudents.Add(student);
                    continue;
                }
                StudentVer2 cloningStudent = (StudentVer2)student.Clone();
                AllStudents.Add(cloningStudent);
                cloningStudents.Add(cloningStudent);
            }
            RefreshItemCount();
            RefreshManagingStudents();
            StudentsSelectionChanged?.Invoke(this, cloningStudents.Cast<object>().ToList());

            if (invalidStudents.Count != 0)
            {
                OnInvalidStudentsTranferred?.Invoke(this, invalidStudents);
            }
        }

        /// <summary>
        /// Gets the event handler for multiple student transfers.
        /// </summary>
        public EventHandler<IList<object>> StudentsTransferHandler => HandleStudentsTransfer;

        /// <summary>
        /// Handles the editing of a student.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The old and new student data.</param>
        public void HandleStudentEdit(object? sender, (object oldItem, object newItem) e)
        {
            if (e.oldItem is not StudentVer2 oldStudent || e.newItem is not StudentVer2 newStudent)
            {
                return;
            }
            if (sender is null)
            {
                return;
            }
            var existingStudent = AllStudents.FirstOrDefault(s => s?.Id == oldStudent.Id, null);
            if (existingStudent == null)
            {
                return;
            }
            if (newStudent.BirthDate.Date == DateTimeToStringConverter.ERROR_DATETIME.Date)
            {
                newStudent.BirthDate = oldStudent.BirthDate;
            }
            existingStudent.Copy(newStudent);
            RefreshManagingStudents();
        }

        /// <summary>
        /// Gets the event handler for student edits.
        /// </summary>
        EventHandler<(object oldItem, object newItem)> ITableItemProvider.ItemEdittedHandler => HandleStudentEdit;

        /// <summary>
        /// Handles the removal of students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The list of students being removed.</param>
        public void HandleStudentsRemoval(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            for (int i = AllStudents.Count - 1; i >= 0; i--)
            {
                var currentStudent = AllStudents[i];
                if (e.Contains(currentStudent))
                {
                    AllStudents.RemoveAt(i);
                }
            }
            RefreshItemCount();
            RefreshManagingStudents();
        }

        /// <summary>
        /// Gets the event handler for student removals.
        /// </summary>
        public EventHandler<IList<object>> StudentsRemoveHandler => HandleStudentsRemoval;

        /// <summary>
        /// Event triggered when the selection of students changes.
        /// </summary>
        public event EventHandler<IList<object>>? StudentsSelectionChanged;
        private int _nextIdForCreation = -1;

        /// <summary>
        /// Handles the creation of new students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public void HandleStudentsCreation(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            var emptyStudent = StudentVer2.Empty();
            emptyStudent.Id = _nextIdForCreation;
            _nextIdForCreation--;
            emptyStudent.RevalidateAllProperties();

            AllStudents.Add(emptyStudent);
            RefreshItemCount();
            RefreshManagingStudents();

            var emptyStudentNewIndex = AllStudents
                .Select((student, index) => new { student, index })
                .First(x => ReferenceEquals(x.student, emptyStudent))
                .index;
            var emptyStudentPage = (emptyStudentNewIndex + 1) / RowsPerPage + (((emptyStudentNewIndex + 1) % RowsPerPage > 0) ? 1 : 0);
            if (emptyStudentPage != CurrentPage)
            {
                NavigateToPage(emptyStudentPage);
            }
            StudentsSelectionChanged?.Invoke(this, [emptyStudent]);
        }

        /// <summary>
        /// Gets the event handler for student creation.
        /// </summary>
        public EventHandler StudentsCreationHandler => HandleStudentsCreation;

        /// <summary>
        /// Event triggered when students are updated.
        /// </summary>
        public event EventHandler<(
            IList<StudentVer2> updatedStudents,
            IList<(StudentVer2 student, IEnumerable<String> errors)> invalidStudentsInfo
            )>? OnStudentsUpdated;

        /// <summary>
        /// Handles the update of students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The list of students being updated.</param>
        public void HandleStudentsUpdate(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            List<(StudentVer2 student, IEnumerable<String> errors)> invalidStudentsInfo = [];
            List<StudentVer2> validAddingStudents = [];
            List<StudentVer2> validUpdatingStudents = [];
            foreach (StudentVer2 updatingStudent in e.Cast<StudentVer2>())
            {
                updatingStudent.RevalidateAllProperties();
                if ((updatingStudent as INotifyDataErrorInfoExtended).HasErrors)
                {
                    invalidStudentsInfo.Add((updatingStudent, []));
                    continue;
                }
                if (updatingStudent.Id < 0)
                {
                    validAddingStudents.Add(updatingStudent);
                }
                else
                {
                    validUpdatingStudents.Add(updatingStudent);
                }
            }
            var (addedStudents, addedCount, invalidAddingStudents) = _dao.AddStudents(validAddingStudents);
            var (updatedStudents, updatedCount, invalidUpdatingStudents) = _dao.UpdateStudents(validUpdatingStudents);

            invalidStudentsInfo.AddRange(invalidAddingStudents);
            invalidStudentsInfo.AddRange(invalidUpdatingStudents);

            var invalidStudents = invalidStudentsInfo.Select(x => x.student);
            var modifiedStudents = e.Cast<StudentVer2>().Where(x => !invalidStudents.Contains(x)).ToList() ?? [];
            OnStudentsUpdated?.Invoke(this, (modifiedStudents, invalidStudentsInfo));
        }

        /// <summary>
        /// Gets the event handler for student updates.
        /// </summary>
        public EventHandler<IList<object>> StudentsUpdateHandler => HandleStudentsUpdate;

        /// <summary>
        /// Handles the deletion of students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The list of students being deleted.</param>
        public void HandleStudentsDelete(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            var (deletedStudents, deletedCount, invalidStudentsInfo) = _dao.DeleteStudents(e.Cast<StudentVer2>());
            OnStudentsUpdated?.Invoke(this, (deletedStudents, invalidStudentsInfo));
        }

        /// <summary>
        /// Gets the event handler for student deletions.
        /// </summary>
        public EventHandler<IList<object>> StudentsDeleteHandler => HandleStudentsDelete;

        /// <summary>
        /// Handles the addition of errors to students.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The list of students and their errors.</param>
        public void HandleStudentsErrorsAdded(object? sender, IList<(StudentVer2 student, IEnumerable<String> errors)> e)
        {
            if (sender is null)
            {
                return;
            }
            foreach (var (student, errors) in e)
            {
                var foundReference = AllStudents.FirstOrDefault(s => ReferenceEquals(student, s), null);
                foundReference?.ChangeErrors("database", errors.ToList() ?? []);
            }
            RefreshManagingStudents();
        }

        /// <summary>
        /// Gets the event handler for adding errors to students.
        /// </summary>
        public EventHandler<IList<(StudentVer2 student, IEnumerable<String> errors)>> StudentsErrorsAddedHandler => HandleStudentsErrorsAdded;

        /// <summary>
        /// Gets the message for the specified item.
        /// </summary>
        /// <param name="item">The item to get the message for.</param>
        /// <returns>The message for the item.</returns>
        InfoBarMessage? IInfoProvider.GetMessageOf(object item)
        {
            if (item is StudentVer2 student)
            {
                if ((student as INotifyDataErrorInfoExtended).HasErrors)
                {
                    var errors = student.GetErrors(null) as List<String> ?? [];
                    var newMessage = String.Join("\n", errors);
                    return new()
                    {
                        Title = (student.Id > 0) ? $"Error with student Id {student.Id}" : $"Error with the newly created student",
                        Message = newMessage,
                        Severity = InfoBarMessageSeverity.Error
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the row status for the specified item.
        /// </summary>
        /// <param name="item">The item to get the row status for.</param>
        /// <returns>The row status for the item.</returns>
        public RowStatus? GetRowStatus(object item)
        {
            if (item is StudentVer2 student)
            {
                if ((student as INotifyDataErrorInfoExtended).HasErrors)
                {
                    return RowStatus.Error;
                }
                if (student.Id == -1)
                {
                    return RowStatus.New;
                }
                return RowStatus.Normal;
            }
            return null;
        }
    }
}

