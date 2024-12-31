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
    /// <summary>
    /// ViewModel for managing and reading student data.
    /// </summary>
    public partial class StudentReaderViewModel(IDao dao) : BaseViewModel, ITableItemProvider, IPagingProvider, ISearchProvider
    {
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
        private readonly IDao _dao = dao;

        /// <summary>
        /// Gets or sets the current page number. Should be non-zero all the time.
        /// </summary>
        public int CurrentPage { get; internal set; } = 1;
        private int _rowsPerPage = DEFAULT_ROWS_PER_PAGE;

        /// <summary>
        /// Gets or sets the number of rows per page. Must be greater than 0.
        /// </summary>
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
        /// Gets the collection of students being managed.
        /// </summary>
        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];

        /// <summary>
        /// Gets the collection of items being managed.
        /// </summary>
        public ObservableCollection<object> ManagingItems { get; private set; } = [];

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        public IEnumerable<string> IgnoringColumns => ["IsValid"];

        /// <summary>
        /// Gets the order of columns.
        /// </summary>
        public IEnumerable<string> ColumnOrder => ["Id", "UserId", "StudentCode", "StudentName", "Email", "BirthDate", "PhoneNo"];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [
            ("BirthDate", new DateTimeToStringConverter()),
                ];

        /// <summary>
        /// Gets the list of managing IDs.
        /// </summary>
        public List<int>? ManagingIds { get; private set; } = null;

        /// <summary>
        /// Gets the list of sort criteria.
        /// </summary>
        public List<SortCriteria>? SortCriteria { get; private set; } = null;

        /// <summary>
        /// Gets the list of search fields.
        /// </summary>
        public List<string> SearchFields
        {
            get => [
                "Id", "UserId", "StudentCode", "StudentName", "Email",
                    "BirthDate", "PhoneNo", "EnrollmentYear", "GraduationYear"
            ];
        }

        /// <summary>
        /// Gets the search criteria.
        /// </summary>
        public SearchCriteria? SearchCriteria { get; private set; } = null;

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
            GetStudents();
            RaisePropertyChanged(nameof(CurrentPage));
            return;
        }

        /// <summary>
        /// Retrieves the students based on the specified IDs and paging settings.
        /// </summary>
        /// <param name="ids">The IDs of the students to retrieve.</param>
        /// <param name="resetPaging">Whether to reset the paging settings.</param>
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
                ManagingItems = new(ManagingStudents);
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

            if (pageChanged)
            {
                RaisePropertyChanged(nameof(CurrentPage));
            }
            RaisePropertyChanged(nameof(ManagingItems));
            RaisePropertyChanged(nameof(ItemCount));
        }

        /// <summary>
        /// Handles changes in sort criteria.
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
            GetStudents();
        }

        /// <summary>
        /// Gets the event handler for sort changes.
        /// </summary>
        public EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;

        /// <summary>
        /// Handles changes in search criteria.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The new search criteria.</param>
        public void HandleSearchChange(object? sender, SearchCriteria? e)
        {
            SearchCriteria = e;
            GetStudents(resetPaging: true);
        }

        /// <summary>
        /// Gets the event handler for search changes.
        /// </summary>
        public EventHandler<SearchCriteria?>? SearchChangedHandler => HandleSearchChange;
    }
}
