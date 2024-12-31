using CloudinaryDotNet.Core;
using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
#nullable enable
using LearningManagementSystem.Models;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    /// <summary>
    /// Delegate for validating a group of items.
    /// </summary>
    /// <param name="validatingItem">The item being validated.</param>
    /// <param name="comparingItems">The items to compare against.</param>
    /// <returns>True if the item is valid, otherwise false.</returns>
    public delegate bool GroupValidator(object validatingItem, IEnumerable<object> comparingItems);

    /// <summary>
    /// Delegate for selecting a group item.
    /// </summary>
    /// <param name="baseItem">The base item.</param>
    /// <param name="comparingItems">The items to compare against.</param>
    /// <returns>The selected item, or null if no item is selected.</returns>
    public delegate object? GroupSelector(object baseItem, IEnumerable<object> comparingItems);

    /// <summary>
    /// Delegate for transforming an item.
    /// </summary>
    /// <param name="baseItem">The base item.</param>
    /// <param name="tranformedItem">The transformed item.</param>
    public delegate void ItemTransformer(object baseItem, ref object tranformedItem);

    /// <summary>
    /// Delegate for checking an item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is valid, otherwise false.</returns>
    public delegate bool ItemChecker(object item);

    /// <summary>
    /// Delegate for modifying items in the data access object (DAO).
    /// </summary>
    /// <param name="items">The items to modify.</param>
    /// <returns>A tuple containing the modified items, the count of modified items, and information about rejected items.</returns>
    public delegate (
        IList<object> modifiedItems,
        int modifiedCount,
        IList<(object rejectedItem, IEnumerable<string> errors)> rejectedItemsInfo
        ) ItemDaoModifier(IEnumerable<object> items);

    /// <summary>
    /// Extended interface for notifying data error information.
    /// </summary>
    public interface INotifyDataErrorInfoExtended : INotifyDataErrorInfo, IValidatableItem
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides

        /// <summary>
        /// Gets the property names that need to be validated.
        /// </summary>
        protected List<string> PropertyNames { get; }

        /// <summary>
        /// Gets the raw errors of the properties. Should not be accessed directly.
        /// </summary>
        protected Dictionary<string, List<string>> RawErrors { get; }

        /// <summary>
        /// Gets the errors of a single property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The errors of the property.</returns>
        public IEnumerable<string> GetErrorsOfSingleProperty(string propertyName);

        /// <summary>
        /// Invokes the ErrorsChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnErrorsChanged(string propertyName);

        /// <summary>
        /// The name of the property indicating no error.
        /// </summary>
        public static readonly string NoErrorPropertyName = "Ehe no error found my lord";

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods

        /// <summary>
        /// Gets the properties of a specified type.
        /// </summary>
        /// <typeparam name="T">The type to get properties of.</typeparam>
        /// <returns>A list of property names.</returns>
        public static List<string> GetPropertiesOf<T>() where T : class
        {
            return typeof(T).GetProperties().Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Validates a single property or all properties if the property name is null or empty.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        void IValidatableItem.ValidateProperty(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !PropertyNames.Contains(propertyName))
            {
                return;
            }
            RawErrors.Remove(propertyName);
            var errors = GetErrorsOfSingleProperty(propertyName);
            if (errors.Any())
            {
                RawErrors.Add(propertyName, errors.ToList());
            }
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Revalidates all properties, ignoring all external errors.
        /// </summary>
        public void RevalidateAllProperties()
        {
            RawErrors.Clear();
            foreach (var propertyName in PropertyNames)
            {
                var errors = GetErrorsOfSingleProperty(propertyName);
                if (errors.Any())
                {
                    RawErrors.Add(propertyName, errors.ToList());
                }
            }
            if (!HasErrors)
            {
                OnErrorsChanged(NoErrorPropertyName);
            }
        }

        /// <summary>
        /// Changes the errors of a property, allowing adding external errors not related to any property in PropertyNames.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="errors">The errors to set.</param>
        public void ChangeErrors(string propertyName, IEnumerable<string> errors)
        {
            RawErrors[propertyName] = errors.ToList();
            if (PropertyNames.Contains(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        // ----------------------------------------------------------------------------------------
        // Internal-used properties and methods

        /// <summary>
        /// Gets a value indicating whether there are any errors.
        /// </summary>
        bool INotifyDataErrorInfo.HasErrors => RawErrors.Count > 0;

        /// <summary>
        /// Gets the errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The errors of the property.</returns>
        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return RawErrors.Values.SelectMany(sublist => sublist);
            }
            return RawErrors.TryGetValue(propertyName, out List<string>? value) ? value : [];
        }
    }

    /// <summary>
    /// Abstract ViewModel class for Create, Update, and Delete (CUD) operations.
    /// </summary>
    /// <param name="dao">Data access object for interacting with the data source.</param>
    abstract class CUDViewModel(IDao dao) : ReaderViewModel(dao), IInfoProvider, IRowStatusDeterminer
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides

        /// <summary>
        /// Gets the message of a specified item.
        /// </summary>
        /// <param name="item">The item to get the message of.</param>
        /// <returns>The message of the item.</returns>
        public abstract InfoBarMessage? GetMessageOf(object item);

        /// <summary>
        /// Gets the row status of a specified item.
        /// </summary>
        /// <param name="item">The item to get the row status of.</param>
        /// <returns>The row status of the item.</returns>
        public abstract RowStatus? GetRowStatus(object item);

        // ----------------------------------------------------------------------------------------
        // Public Delegates

        //public delegate bool GroupValidator(object validatingItem, IEnumerable<object> comparingItems);

        //public delegate object? GroupSelector(object baseItem, IEnumerable<object> comparingItems);

        //public delegate void ItemTransformer(object baseItem, ref object tranformedItem);

        //public delegate bool ItemChecker(object item);

        //public delegate (
        //    IList<object> modifiedItems,
        //    int modifiedCount,
        //    IList<(object rejectedItem, IEnumerable<string> errors)> rejectedItemsInfo
        //    ) ItemDaoModifier(IEnumerable<object> items);

        // ----------------------------------------------------------------------------------------
        // Functional overrides
        // (Those overrides should be given so that the ViewModel can work as expected)

        /// <summary>
        /// Gets the filter for transferring items.
        /// </summary>
        /// <returns>The filter for transferring items.</returns>
        virtual public GroupValidator? TransferingItemFilter => null;

        /// <summary>
        /// Gets the validation for old items when editing.
        /// </summary>
        /// <returns>The validation for old items when editing.</returns>
        virtual public GroupSelector? OldItemValidationOnEditing => null;

        /// <summary>
        /// Gets the transformer for items when editing.
        /// </summary>
        /// <returns>The transformer for items when editing.</returns>
        virtual public ItemTransformer? ItemTransformerOnEditting => null;

        /// <summary>
        /// Gets the transformer for existing items when editing.
        /// </summary>
        /// <returns>The transformer for existing items when editing.</returns>
        virtual public ItemTransformer? ExistingItemTransformerOnEditting => null;

        /// <summary>
        /// Gets the empty item for adding.
        /// </summary>
        /// <param name="identitySeed">The identity seed for the item.</param>
        /// <returns>The empty item for adding.</returns>
        virtual public object? EmptyItem(ref object? identitySeed) => null;

        /// <summary>
        /// Gets the checker for items to update.
        /// </summary>
        /// <returns>The checker for items to update.</returns>
        virtual public ItemChecker? ItemCheckForUpdate => null;

        /// <summary>
        /// Gets the checker for items to add.
        /// </summary>
        /// <returns>The checker for items to add.</returns>
        virtual public ItemChecker? ItemCheckForAdd => null;

        /// <summary>
        /// Gets the modifier for inserting items in the DAO.
        /// </summary>
        /// <returns>The modifier for inserting items in the DAO.</returns>
        virtual public ItemDaoModifier? ItemDaoInserter => null;

        /// <summary>
        /// Gets the modifier for updating items in the DAO.
        /// </summary>
        /// <returns>The modifier for updating items in the DAO.</returns>
        virtual public ItemDaoModifier? ItemDaoUpdater => null;

        /// <summary>
        /// Gets the modifier for deleting items in the DAO.
        /// </summary>
        /// <returns>The modifier for deleting items in the DAO.</returns>
        virtual public ItemDaoModifier? ItemDaoDeleter => null;

        // ----------------------------------------------------------------------------------------
        // Public Events

        /// <summary>
        /// Event triggered when invalid items are transferred.
        /// </summary>
        public event EventHandler<IList<object>>? OnInvalidItemsTranferred;

        /// <summary>
        /// Event triggered when the selection of items changes.
        /// </summary>
        public event EventHandler<IList<object>>? ItemsSelectionChanged;

        /// <summary>
        /// Event triggered when items are updated.
        /// </summary>
        public event EventHandler<(
            IList<object> updatedItems,
            IList<(object item, IEnumerable<string> errors)> invalidItemsInfo
            )>? OnItemsUpdated;

        // ----------------------------------------------------------------------------------------
        // Public Handlers

        /// <summary>
        /// Handler for transferring a single item.
        /// </summary>
        public EventHandler<object> ItemTransferHandler => HandleItemTransfer;

        /// <summary>
        /// Handler for transferring multiple items.
        /// </summary>
        public EventHandler<IList<object>> ItemsTransferHandler => HandleItemsTransfer;

        /// <summary>
        /// Handler for editing an item.
        /// </summary>
        override public EventHandler<(object oldItem, object newItem)> ItemEdittedHandler => HandleItemEdit;

        /// <summary>
        /// Handler for removing items.
        /// </summary>
        public EventHandler<IList<object>> ItemsRemoveHandler => HandleItemsRemoval;

        /// <summary>
        /// Handler for creating an item.
        /// </summary>
        public EventHandler ItemCreationHandler => HandleItemCreation;

        /// <summary>
        /// Handler for updating items.
        /// </summary>
        public EventHandler<IList<object>> ItemsUpdateHandler => HandleItemsUpdate;

        /// <summary>
        /// Handler for deleting items.
        /// </summary>
        public EventHandler<IList<object>> ItemsDeleteHandler => HandleItemsDelete;

        /// <summary>
        /// Handler for adding errors to items.
        /// </summary>
        public EventHandler<IList<(object item, IEnumerable<string> errors)>> ItemsErrorsAddedHandler => HandleItemsErrorsAdded;

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods

        /// <summary>
        /// Populates the items in the ViewModel.
        /// </summary>
        /// <param name="items">The items to populate.</param>
        public void PopulateItems(IEnumerable<object> items)
        {
            AllItems = new ObservableCollection<object>(items);
            RefreshItemCount();
            RefreshManagingItems();
        }

        // ----------------------------------------------------------------------------------------
        // Overrides from ReaderViewModel

        /// <summary>
        /// Navigates to the specified page number.
        /// </summary>
        /// <param name="pageNumber">The page number to navigate to.</param>
        public override void NavigateToPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                return;
            }

            CurrentPage = pageNumber;
            RefreshManagingItems();
            RaisePropertyChanged(nameof(CurrentPage));
        }

        /// <summary>
        /// Loads the initial set of items.
        /// </summary>
        public override void LoadInitialItems()
        {
            // No need to do anything
        }

        /// <summary>
        /// Gets the handler for when the sort criteria changes.
        /// </summary>
        public override EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;

        // ----------------------------------------------------------------------------------------
        // Internal properties and methods

        /// <summary>
        /// Gets the items from the data source.
        /// </summary>
        /// <param name="ignoreCount">The number of items to skip.</param>
        /// <param name="fetchCount">The number of items to fetch.</param>
        /// <param name="sortCriteria">The criteria for sorting the items.</param>
        /// <param name="searchCriteria">The criteria for searching the items.</param>
        /// <returns>A tuple containing the fetched items and the total query count.</returns>
        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            )
        {
            return ([], 0);
        }

        /// <summary>
        /// Gets or sets all items in the ViewModel.
        /// </summary>
        public ObservableCollection<object> AllItems { get; private set; } = [];

        /// <summary>
        /// Refreshes the managing items based on the current page and sort criteria.
        /// </summary>
        public void RefreshManagingItems()
        {
            SortByItsCriteria();
            ManagingItems = new(
                AllItems.AsEnumerable()
                    .Skip((CurrentPage - 1) * RowsPerPage)
                    .Take(RowsPerPage)
            );
            RaisePropertyChanged(nameof(ManagingItems));
        }

        /// <summary>
        /// Refreshes the item count and updates the page count if necessary.
        /// </summary>
        public void RefreshItemCount()
        {
            ItemCount = AllItems.Count;
            RaisePropertyChanged(nameof(ItemCount));
            if (CurrentPage > PageCount)
            {
                NavigateToPage(PageCount);
            }
        }

        /// <summary>
        /// Sorts the items by the specified criteria.
        /// </summary>
        private void SortByItsCriteria()
        {
            if (SortCriteria is null || SortCriteria.Count == 0)
            {
                return;
            }
            IEnumerable<object> sortResult = AllItems;
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
            AllItems = new ObservableCollection<object>(sortResult);
        }

        /// <summary>
        /// Handles changes in the sort criteria.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The new sort criteria.</param>
        public new void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            RefreshManagingItems();
        }
        /// <summary>
        /// Handles the transfer of a single item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The item to be transferred.</param>
        public void HandleItemTransfer(object? sender, object e)
        {
            HandleItemsTransfer(sender, [e]);
        }
        /// <summary>
        /// Handles the transfer of multiple items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The list of items to be transferred.</param>
        public void HandleItemsTransfer(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            List<object> invalidItems = [];
            List<object> cloningItems = [];
            foreach (object item in e)
            {
                if (TransferingItemFilter != null && !TransferingItemFilter(item, AllItems))
                {
                    invalidItems.Add(item);
                    continue;
                }

                if (item is not System.ICloneable)
                {
                    Debug.Assert(false, "The item should be implemented with ICloneable interface for the handler to work properly");
                }

                object cloningItem = item is System.ICloneable clonable ? clonable.Clone() : item;
                AllItems.Add(cloningItem);
                cloningItems.Add(cloningItem);
            }
            RefreshItemCount();
            RefreshManagingItems();
            ItemsSelectionChanged?.Invoke(this, cloningItems);

            if (invalidItems.Count != 0)
            {
                OnInvalidItemsTranferred?.Invoke(this, invalidItems);
            }
        }
        /// <summary>
        /// Handles the editing of an item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A tuple containing the old item and the new item.</param>
        public void HandleItemEdit(object? sender, (object oldItem, object newItem) e)
        {
            if (sender is null)
            {
                return;
            }

            if (OldItemValidationOnEditing == null)
            {
                return;
            }

            var existingItem = OldItemValidationOnEditing?.Invoke(e.oldItem, AllItems);
            if (existingItem == null)
            {
                return;
            }

            ItemTransformerOnEditting?.Invoke(e.oldItem, ref e.newItem);
            ExistingItemTransformerOnEditting?.Invoke(e.newItem, ref existingItem);
            RefreshManagingItems();
        }
        /// <summary>
        /// Handles the removal of multiple items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The list of items to be removed.</param>
        public void HandleItemsRemoval(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            for (int i = AllItems.Count - 1; i >= 0; i--)
            {
                var currentItem = AllItems[i];
                if (e.Contains(currentItem))
                {
                    AllItems.RemoveAt(i);
                }
            }
            RefreshItemCount();
            RefreshManagingItems();
        }
        /// <summary>
        /// Handles the creation of a new item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private object? identitySeedHolder = null;
        public void HandleItemCreation(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            var emptyItem = EmptyItem(ref identitySeedHolder);
            if (emptyItem is null)
            {
                return;
            }

            if (emptyItem is INotifyDataErrorInfoExtended errorNotifiable)
            {
                errorNotifiable.RevalidateAllProperties();
            }

            AllItems.Add(emptyItem);
            RefreshItemCount();
            RefreshManagingItems();

            var emptyItemNewIndex = AllItems
                .Select((item, index) => new { item, index })
                .First(x => ReferenceEquals(x.item, emptyItem))
                .index;
            var emptyItemPage = (emptyItemNewIndex + 1) / RowsPerPage + (((emptyItemNewIndex + 1) % RowsPerPage > 0) ? 1 : 0);
            if (emptyItemPage != CurrentPage)
            {
                NavigateToPage(emptyItemPage);
            }
            ItemsSelectionChanged?.Invoke(this, [emptyItem]);
        }

        /// <summary>
        /// Handles the update of multiple items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The list of items to be updated.</param>
        public void HandleItemsUpdate(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            List<(object item, IEnumerable<string> errors)> invalidItemsInfo = [];
            List<object> validAddingItems = [];
            List<object> validUpdatingItems = [];

            foreach (object updatingItem in e)
            {
                if (updatingItem is INotifyDataErrorInfoExtended errorNotifier)
                {
                    errorNotifier.RevalidateAllProperties();
                    if (errorNotifier.HasErrors)
                    {
                        invalidItemsInfo.Add((errorNotifier, []));
                        continue;
                    }
                }

                if (ItemCheckForAdd != null && ItemCheckForAdd(updatingItem))
                {
                    validAddingItems.Add(updatingItem);
                }
                else if (ItemCheckForUpdate != null && ItemCheckForUpdate(updatingItem))
                {
                    validUpdatingItems.Add(updatingItem);
                }
                else
                {
                    invalidItemsInfo.Add(
                        (updatingItem, ["This item is not allowed to be inserted or updated"])
                        );
                }
            }
            var (addedItems, addedCount, invalidAddingItems) = ItemDaoInserter?.Invoke(validAddingItems)
                ?? (
                new List<object>(),
                0,
                new List<(object, IEnumerable<string>)>(validAddingItems.Select(x =>
                    {
                        var rejectedItem = x;
                        IEnumerable<string> errors = ["This item is not allowed to be inserted"];
                        return (rejectedItem, errors);
                    }))
                );
            var (updatedItems, updatedCount, invalidUpdatingItems) = ItemDaoUpdater?.Invoke(validUpdatingItems)
                ?? (
                new List<object>(),
                0,
                new List<(object, IEnumerable<string>)>(validUpdatingItems.Select(x =>
                    {
                        var rejectedItem = x;
                        IEnumerable<string> errors = ["This item is not allowed to be updated"];
                        return (rejectedItem, errors);
                    }))
                );

            invalidItemsInfo.AddRange(invalidAddingItems);
            invalidItemsInfo.AddRange(invalidUpdatingItems);

            var invalidItems = invalidItemsInfo.Select(x => x.item);
            var modifiedItems = e.Where(x => !invalidItems.Contains(x)).ToList() ?? [];
            OnItemsUpdated?.Invoke(this, (modifiedItems, invalidItemsInfo));
        }
        /// <summary>
        /// Handles the deletion of multiple items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The list of items to be deleted.</param>
        public void HandleItemsDelete(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            var (deletedItems, deletedCount, invalidItemsInfo) = ItemDaoDeleter?.Invoke(e)
                ?? (
                new List<object>(),
                0,
                new List<(object, IEnumerable<string>)>(e.Select(x =>
                    {
                        var rejectedItem = x;
                        IEnumerable<string> errors = ["This item is not allowed to be deleted"];
                        return (rejectedItem, errors);
                    }))
                );

            OnItemsUpdated?.Invoke(this, (deletedItems, invalidItemsInfo));
        }

        /// <summary>
        /// Handles the addition of errors to multiple items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The list of items and their associated errors.</param>
        public void HandleItemsErrorsAdded(object? sender, IList<(object item, IEnumerable<string> errors)> e)
        {
            if (sender is null)
            {
                return;
            }
            foreach (var (item, errors) in e)
            {
                var foundReference = AllItems.FirstOrDefault(s => ReferenceEquals(item, s), null);
                if (foundReference is INotifyDataErrorInfoExtended errorNotifier)
                {
                    errorNotifier.ChangeErrors("database", errors.ToList() ?? []);
                }
            }
            RefreshManagingItems();
        }
    }
}
