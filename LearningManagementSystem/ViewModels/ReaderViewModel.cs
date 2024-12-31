#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using Microsoft.UI.Xaml.Data;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// Abstract ViewModel class for reading and managing items with support for paging, sorting, and searching.
    /// </summary>
    /// <param name="dao">Data access object for interacting with the data source.</param>
    abstract class ReaderViewModel(IDao dao) : BaseViewModel, ITableItemProvider, IPagingProvider, ISearchProvider
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides

        /// <summary>
        /// Fetches items from the data source based on the specified parameters.
        /// </summary>
        /// <param name="ignoreCount">Number of items to skip.</param>
        /// <param name="fetchCount">Number of items to fetch.</param>
        /// <param name="sortCriteria">Criteria for sorting the items.</param>
        /// <param name="searchCriteria">Criteria for searching the items.</param>
        /// <returns>A tuple containing the fetched items and the total query count.</returns>
        abstract protected (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            );

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods

        /// <summary>
        /// Loads the initial set of items.
        /// </summary>
        virtual public void LoadInitialItems()
        {
            GetItemsInternal();
        }

        // ----------------------------------------------------------------------------------------
        // Constructor
        protected readonly IDao _dao = dao;

        // ----------------------------------------------------------------------------------------
        // IPagingProvider

        /// <summary>
        /// Default number of rows per page.
        /// </summary>
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;

        /// <summary>
        /// Gets or sets the current page number.
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
        public int ItemCount { get; protected set; } = 0;

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int PageCount { get => ItemCount / RowsPerPage + ((ItemCount % RowsPerPage > 0) ? 1 : 0); }

        /// <summary>
        /// Navigates to the specified page number.
        /// </summary>
        /// <param name="pageNumber">The page number to navigate to.</param>
        virtual public void NavigateToPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                return;
            }

            CurrentPage = pageNumber;

            GetItemsInternal();

            RaisePropertyChanged(nameof(CurrentPage));
            return;
        }

        // ----------------------------------------------------------------------------------------
        // ITableItemProvider

        /// <summary>
        /// Gets the collection of items being managed.
        /// </summary>
        public ObservableCollection<object> ManagingItems { get; protected set; } = [];

        /// <summary>
        /// Event handler for when the sort criteria changes.
        /// </summary>
        virtual public EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;

        /// <summary>
        /// Event handler for when an item is double-tapped.
        /// </summary>
        virtual public EventHandler<object>? ItemDoubleTappedHandler => null;

        /// <summary>
        /// Event handler for when an item is edited.
        /// The item should be implemented with ICloneable interface. If not, the oldItem will be the same as newItem.
        /// </summary>
        virtual public EventHandler<(object oldItem, object newItem)>? ItemEdittedHandler => null;

        /// <summary>
        /// Gets the columns to be ignored.
        /// </summary>
        virtual public IEnumerable<string> IgnoringColumns => [];

        /// <summary>
        /// Gets the order of the columns.
        /// </summary>
        virtual public IEnumerable<string> ColumnOrder => [];

        /// <summary>
        /// Gets the column converters.
        /// </summary>
        virtual public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];

        /// <summary>
        /// Gets the read-only columns.
        /// </summary>
        virtual public IEnumerable<string> ReadOnlyColumns => [];

        // ----------------------------------------------------------------------------------------
        // ISearchProvider

        /// <summary>
        /// Gets the fields available for searching.
        /// </summary>
        virtual public List<string> SearchFields { get; } = [];

        /// <summary>
        /// Event handler for when the search criteria changes.
        /// </summary>
        virtual public EventHandler<SearchCriteria?>? SearchChangedHandler => HandleSearchChange;

        // ----------------------------------------------------------------------------------------
        // Internal properties and methods

        /// <summary>
        /// Gets or sets the current sort criteria.
        /// </summary>
        public List<SortCriteria>? SortCriteria { get; protected set; } = null;

        /// <summary>
        /// Gets or sets the current search criteria.
        /// </summary>
        public SearchCriteria? SearchCriteria { get; protected set; } = null;

        /// <summary>
        /// Fetches items internally with optional paging reset.
        /// </summary>
        /// <param name="resetPaging">Indicates whether to reset paging.</param>
        private void GetItemsInternal(bool resetPaging = false)
        {
            if (resetPaging)
            {
                CurrentPage = 1;
            }

            bool pageChanged = false;
            do
            {
                var (result, queryCount) = GetItems(
                    ignoreCount: (CurrentPage - 1) * RowsPerPage,
                    fetchCount: RowsPerPage,
                    sortCriteria: SortCriteria,
                    searchCriteria: SearchCriteria
                    );
                ItemCount = queryCount;
                ManagingItems = new(result);
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
            RaisePropertyChanged(nameof(ManagingItems));
            RaisePropertyChanged(nameof(ItemCount));
        }

        /// <summary>
        /// Handles changes in the sort criteria.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The new sort criteria.</param>
        public void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            GetItemsInternal();
        }

        /// <summary>
        /// Handles changes in the search criteria.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The new search criteria.</param>
        public void HandleSearchChange(object? sender, SearchCriteria? e)
        {
            SearchCriteria = e;
            GetItemsInternal(resetPaging: true);
        }
    }
}
