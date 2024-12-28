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
    public partial class StudentCUDViewModel: BaseViewModel, ITableItemProvider, IPagingProvider, IInfoProvider, IRowStatusDeterminer, IDisposable
    {
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
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
        private readonly IDao _dao;

        /// <summary>
        /// Should be non-zero all the time
        /// </summary>
        public int CurrentPage { get; internal set; } = 1;
        private int _rowsPerPage = DEFAULT_ROWS_PER_PAGE;

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

        [AlsoNotifyFor(nameof(PageCount))]
        public int ItemCount { get; private set; } = 0;
        public int PageCount { get => ItemCount / RowsPerPage + ((ItemCount % RowsPerPage > 0) ? 1 : 0); }
        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];
        public ObservableCollection<object> ManagingItems { get; private set; } = [];
        public IEnumerable<string> IgnoringColumns => [];
        public IEnumerable<string> ColumnOrder => ["Id", "UserId", "IsValid", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo", "EnrollmentYear", "GraduationYear"];
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("Id", new NegativeIntToNewMarkerConverter()),
            ("UserId", new NegativeIntToNewMarkerConverter()),
            ("GraduationYear", new NullableIntToStringConverter()),
            ("BirthDate", new DateTimeToStringConverter()),
            ];
        public IEnumerable<string> ReadOnlyColumns => ["Id", "UserId", "IsValid"];
        public ObservableCollection<StudentVer2> AllStudents { get; private set; } = [];
        public List<SortCriteria>? SortCriteria { get; private set; } = null;

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

        public void RefreshItemCount()
        {
            ItemCount = AllStudents.Count;
            RaisePropertyChanged(nameof(ItemCount));
            if (CurrentPage > PageCount)
            {
                NavigateToPage(PageCount);
            }
        }

        public void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            RefreshManagingStudents();
        }
        public EventHandler<List<SortCriteria>> SortChangedHandler => HandleSortChange;

        public event EventHandler<IList<StudentVer2>>? OnInvalidStudentsTranferred;
        public void HandleStudentTransfer(object? sender, object e)
        {
            HandleStudentsTransfer(sender, [e]);
        }
        public EventHandler<object> StudentTransferHandler => HandleStudentTransfer;

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
        public EventHandler<IList<object>> StudentsTransferHandler => HandleStudentsTransfer;

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
        EventHandler<(object oldItem, object newItem)> ITableItemProvider.ItemEdittedHandler => HandleStudentEdit;

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
        
        public EventHandler<IList<object>> StudentsRemoveHandler => HandleStudentsRemoval;

        public event EventHandler<IList<object>>? StudentsSelectionChanged;
        private int _nextIdForCreation = -1;
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
        public EventHandler StudentsCreationHandler => HandleStudentsCreation;

        public event EventHandler<(
            IList<StudentVer2> updatedStudents,
            IList<(StudentVer2 student, IEnumerable<String> errors)> invalidStudentsInfo
            )>? OnStudentsUpdated;
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

            //invalidStudentsInfo.AddRange(
            //    validAddingStudents
            //        .Where(x => addedStudents.Where(a => a.Equals(x)).Any())
            //        .Select(x => {
            //            (StudentVer2 student, IEnumerable<String> errors) = (x, ["Added failed in database"]);
            //            return (student, errors);
            //            })
            //    );
            invalidStudentsInfo.AddRange(invalidAddingStudents);
            invalidStudentsInfo.AddRange(invalidUpdatingStudents);

            var invalidStudents = invalidStudentsInfo.Select(x => x.student);
            var modifiedStudents = e.Cast<StudentVer2>().Where(x => !invalidStudents.Contains(x)).ToList() ?? [];
            OnStudentsUpdated?.Invoke(this, (modifiedStudents, invalidStudentsInfo));
        }
        public EventHandler<IList<object>> StudentsUpdateHandler => HandleStudentsUpdate;

        public void HandleStudentsDelete(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            var (deletedStudents, deletedCount, invalidStudentsInfo) = _dao.DeleteStudents(e.Cast<StudentVer2>());
            //for (int i = AllStudents.Count - 1; i >= 0; i--)
            //{
            //    var currentStudent = AllStudents[i];
            //    if (deletedStudents.Contains(currentStudent))
            //    {
            //        AllStudents.RemoveAt(i);
            //    }
            //}
            //RefreshItemCount();
            //RefreshManagingStudents();
            OnStudentsUpdated?.Invoke(this, (deletedStudents, invalidStudentsInfo));
        }
        public EventHandler<IList<object>> StudentsDeleteHandler => HandleStudentsDelete;

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
        public EventHandler<IList<(StudentVer2 student, IEnumerable<String> errors)>> StudentsErrorsAddedHandler => HandleStudentsErrorsAdded;

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

