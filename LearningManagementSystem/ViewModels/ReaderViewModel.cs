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
    abstract class ReaderViewModel(IDao dao): BaseViewModel, ITableItemProvider, IPagingProvider, ISearchProvider
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides
        abstract protected (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            );

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods
        virtual public void LoadInitialItems()
        {
            GetItemsInternal();
        }

        // ----------------------------------------------------------------------------------------
        // Constructor
        protected readonly IDao _dao = dao;

        // ----------------------------------------------------------------------------------------
        // IPagingProvider
        public static readonly int DEFAULT_ROWS_PER_PAGE = 10;
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
        public int ItemCount { get; protected set; } = 0;
        public int PageCount { get => ItemCount / RowsPerPage + ((ItemCount % RowsPerPage > 0) ? 1 : 0); }
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
        public ObservableCollection<object> ManagingItems { get; protected set; } = [];
        virtual public EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;
        virtual public EventHandler<object>? ItemDoubleTappedHandler => null;

        /// <summary>
        /// The item should be implemented with ICloneable interface. If not, the oldItem will be the same as newItem.
        /// </summary>
        virtual public EventHandler<(object oldItem, object newItem)>? ItemEdittedHandler => null;
        virtual public IEnumerable<string> IgnoringColumns => [];
        virtual public IEnumerable<string> ColumnOrder => [];
        virtual public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];
        virtual public IEnumerable<string> ReadOnlyColumns => [];

        // ----------------------------------------------------------------------------------------
        // ISearchProvider
        virtual public List<string> SearchFields { get; } = [];
        virtual public EventHandler<SearchCriteria?>? SearchChangedHandler => HandleSearchChange;

        // ----------------------------------------------------------------------------------------
        // Internal properties and methods
        public List<SortCriteria>? SortCriteria { get; protected set; } = null;
        public SearchCriteria? SearchCriteria { get; protected set; } = null;

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

        public void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            GetItemsInternal();
        }

        public void HandleSearchChange(object? sender, SearchCriteria? e)
        {
            SearchCriteria = e;
            GetItemsInternal(resetPaging: true);
        }
    }
}
