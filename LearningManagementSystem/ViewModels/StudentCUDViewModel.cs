﻿#nullable enable
using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public partial class StudentCUDViewModel(IDao dao): BaseViewModel, IStudentProvider, IPagingProvider
    {
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
        private readonly IDao _dao = dao;

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
            RaisePropertyChanged(nameof(ManagingStudents));
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
        public EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;

        public event EventHandler<StudentVer2>? InvalidTransferingStudentHandler;
        public void HandleStudentTransfer(object? sender, StudentVer2 e)
        {
            if (sender is null)
            {
                return;
            }
            var existingStudent = AllStudents.FirstOrDefault(s => s?.Id == e.Id, null);
            if (existingStudent != null)
            {
                InvalidTransferingStudentHandler?.Invoke(this, e);
                return;
            }
            AllStudents.Add((StudentVer2)e.Clone());
            ItemCount++;
            RaisePropertyChanged(nameof(ItemCount));

            RefreshManagingStudents();
        }
        public EventHandler<StudentVer2> StudentTransferHandler => HandleStudentTransfer;

        public void HandleStudentEdit(object? sender, (StudentVer2 oldStudent, StudentVer2 newStudent) e)
        {
            if (sender is null)
            {
                return;
            }
            var existingStudent = AllStudents.FirstOrDefault(s => s?.Id == e.oldStudent.Id, null);
            if (existingStudent == null)
            {
                return;
            }
            if (e.newStudent.BirthDate.Date == DateTimeToStringConverter.ERROR_DATETIME.Date)
            {
                e.newStudent.BirthDate = e.oldStudent.BirthDate;
            }
            existingStudent.Copy(e.newStudent);
            RefreshManagingStudents();
        }
        public EventHandler<(StudentVer2 oldStudent, StudentVer2 newStudent)>? StudentEdittedHandler => HandleStudentEdit;
    }
}

