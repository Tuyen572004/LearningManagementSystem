#nullable enable
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
    public partial class StudentReaderViewModel(IDao dao) : BaseViewModel, IStudentProvider, IPagingProvider, ISearchProvider
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
        public IEnumerable<string> IgnoringColumns => ["HasErrors"];
        public IEnumerable<string> ColumnOrder => ["Id", "UserId", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("BirthDate", new DateTimeToStringConverter()),
            ];
        public List<int>? ManagingIds { get; private set; } = null;
        public List<SortCriteria>? SortCriteria { get; private set; } = null;
        public List<string> SearchFields {
            get => [
                "Id", "UserId", "StudentCode", "StudentName", "Email",
                "BirthDate", "PhoneNo", "EnrollmentYear", "GraduationYear"
            ];
        }
        public SearchCriteria? SearchCriteria { get; private set; } = null;

        public void NavigateToPage(int pageNumber)
        {
            //if (pageNumber < 1 || pageNumber > PageCount)
            //{
            //    return;
            //}

            if (pageNumber < 1)
            {
                return;
            }

            CurrentPage = pageNumber;
            GetStudents();

            //if (CurrentPage > PageCount + 1)
            //{
            //    CurrentPage = PageCount + 1;
            //}

            RaisePropertyChanged(nameof(CurrentPage));
            return;
        }
        public void GetStudents(IEnumerable<int>? ids = null, bool resetPaging = false)
        {
            if (ids != null)
            {
                ManagingIds = ids.ToList();
            }

            if (resetPaging)
            {
                CurrentPage = 1;
            }

            // ManagingIds = ids?.ToList();
            //var (resultList, queryCount) = _dao.GetStudentsById(
            //    ignoringCount: (CurrentPage - 1) * RowsPerPage,
            //    fetchingCount: RowsPerPage,
            //    chosenIds: ids
            //    );

            bool pageChanged = false;
            do
            {
                var (resultList, queryCount) = _dao.GetStudents(
                ignoringCount: (CurrentPage - 1) * RowsPerPage,
                fetchingCount: RowsPerPage,
                chosenIds: ManagingIds,
                sortCriteria: SortCriteria,
                searchCriteria: SearchCriteria
                );
                ItemCount = queryCount;
                ManagingStudents = resultList;
                // Why the do loop and this check:
                // If the current page is empty, "queryCount" would always be zero, regardless of the actual number of items in the database.
                if (ItemCount == 0 && CurrentPage > 1)
                {
                    CurrentPage--;
                    pageChanged = true;
                }
                else
                {
                    break;
                }
            }
            while (true);

            // You must "roar" by yourself :'))
            if (pageChanged)
            {
                RaisePropertyChanged(nameof(CurrentPage));
            }
            RaisePropertyChanged(nameof(ManagingStudents));
            RaisePropertyChanged(nameof(ItemCount));
        }

        public void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            GetStudents();
        }
        public EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;
        public void HandleSearchChange(object? sender, SearchCriteria? e)
        {
            SearchCriteria = e;
            GetStudents(resetPaging: true);
        }
        public EventHandler<SearchCriteria?>? SearchChangedHandler => HandleSearchChange;
    }
}
