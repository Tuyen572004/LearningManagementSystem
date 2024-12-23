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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public delegate bool GroupValidator(object validatingItem, IEnumerable<object> comparingItems);

    public delegate object? GroupSelector(object baseItem, IEnumerable<object> comparingItems);

    public delegate void ItemTransformer(object baseItem, ref object tranformedItem);

    public delegate bool ItemChecker(object item);

    public delegate (
        IList<object> modifiedItems,
        int modifiedCount,
        IList<(object rejectedItem, IEnumerable<string> errors)> rejectedItemsInfo
        ) ItemDaoModifier(IEnumerable<object> items);
    public interface INotifyDataErrorInfoExtended: INotifyDataErrorInfo, IValidatableItem
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides

        // Only contains the property names that are needed to be validated
        protected List<string> PropertyNames { get; }

        // Property to store the raw errors of the properties, should not be accessed directly
        protected Dictionary<string, List<string>> RawErrors { get; }

        /// Get the errors of a single property
        public IEnumerable<string> GetErrorsOfSingleProperty(string propertyName);

        // Required in INotifyDataErrorInfo
        // public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // Should invoke ErrorsChanged event (possibly reject the event if the propertyName is not in PropertyNames)
        protected void OnErrorsChanged(string propertyName);
        public static readonly string NoErrorPropertyName = "Ehe no error found my lord";

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods
        public static List<string> GetPropertiesOf<T>() where T : class
        {
            return typeof(T).GetProperties().Select(p => p.Name).ToList();
        }

        // Validate a single property or all properties (if propertyName is null or empty)
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

        // Revalidate all properties, ignoring all external errors
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

        // Change the errors of a property; allow adding external errors that is not related to any property in PropertyNames
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
        bool INotifyDataErrorInfo.HasErrors => RawErrors.Count > 0;

        IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return RawErrors.Values.SelectMany(sublist => sublist);
            }
            return RawErrors.TryGetValue(propertyName, out List<string>? value) ? value : [];
        }
    }

    abstract class CUDViewModel(IDao dao): ReaderViewModel(dao), IInfoProvider, IRowStatusDeterminer
    {
        // ----------------------------------------------------------------------------------------
        // Required overrides

        public abstract InfoBarMessage? GetMessageOf(object item);
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
        /// Decide whether the item should be transfered to the managing items or not, base on the current managing items
        /// </summary>
        virtual public GroupValidator? TransferingItemFilter => null;

        /// <summary>
        /// Check if the editted item is in the managing items
        /// </summary>
        virtual public GroupSelector? OldItemValidationOnEditing => null;

        /// <summary>
        /// Transform the editted item before updating the managing items
        /// </summary>
        virtual public ItemTransformer? ItemTransformerOnEditting => null;

        /// <summary>
        /// Update the existing item with the new item
        /// </summary>
        virtual public ItemTransformer? ExistingItemTransformerOnEditting => null;

        /// <summary>
        /// Return the empty item for adding
        /// </summary>
        /// <returns></returns>
        virtual public object? EmptyItem() => null;

        virtual public ItemChecker? ItemCheckForUpdate => null;
        virtual public ItemChecker? ItemCheckForAdd => null;
        virtual public ItemDaoModifier? ItemDaoInserter => null;
        virtual public ItemDaoModifier? ItemDaoUpdater => null;
        virtual public ItemDaoModifier? ItemDaoDeleter => null;

        // ----------------------------------------------------------------------------------------
        // Public Events
        public event EventHandler<IList<object>>? OnInvalidItemsTranferred;

        public event EventHandler<IList<object>>? ItemsSelectionChanged;

        public event EventHandler<(
            IList<object> updatedItems,
            IList<(object item, IEnumerable<string> errors)> invalidItemsInfo
            )>? OnItemsUpdated;

        // ----------------------------------------------------------------------------------------
        // Public Handlers
        public EventHandler<ICloneable> ItemTransferHandler => HandleItemTransfer;
        public EventHandler<IList<ICloneable>> ItemsTransferHandler => HandleItemsTransfer;
        override public EventHandler<(object oldItem, object newItem)> ItemEdittedHandler => HandleItemEdit;
        public EventHandler<IList<object>> ItemsRemoveHandler => HandleItemsRemoval;
        public EventHandler ItemsCreationHandler => HandleItemsCreation;
        public EventHandler<IList<object>> ItemsUpdateHandler => HandleItemsUpdate;
        public EventHandler<IList<object>> ItemsDeleteHandler => HandleItemsDelete;
        public EventHandler<IList<(object item, IEnumerable<string> errors)>> ItemsErrorsAddedHandler => HandleItemsErrorsAdded;

        // ----------------------------------------------------------------------------------------
        // Useful properties and methods

        // ----------------------------------------------------------------------------------------
        // Overrides from ReaderViewModel

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

        public override void LoadInitialItems()
        {
            // No need to do anything
        }

        public override EventHandler<List<SortCriteria>>? SortChangedHandler => HandleSortChange;

        // ----------------------------------------------------------------------------------------
        // Internal properties and methods

        protected override (ObservableCollection<object> fetchedItem, int queryCount) GetItems(
            int ignoreCount = 0,
            int fetchCount = 0,
            List<SortCriteria>? sortCriteria = null,
            SearchCriteria? searchCriteria = null
            )
        {
            return ([], 0);
        }

        public ObservableCollection<object> AllItems { get; private set; } = [];

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

        public void RefreshItemCount()
        {
            ItemCount = AllItems.Count;
            RaisePropertyChanged(nameof(ItemCount));
            if (CurrentPage > PageCount)
            {
                NavigateToPage(PageCount);
            }
        }

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

        public new void HandleSortChange(object? sender, List<SortCriteria>? e)
        {
            if (sender is null)
            {
                return;
            }
            SortCriteria = e;
            RefreshManagingItems();
        }
        // ----------------------------------------------------------------------------------------
        // Transfering Handling

        public void HandleItemTransfer(object? sender, ICloneable e)
        {
            HandleItemsTransfer(sender, [e]);
        }
        public void HandleItemsTransfer(object? sender, IList<ICloneable> e)
        {
            if (sender is null)
            {
                return;
            }
            List<object> invalidItems = [];
            List<object> cloningItems = [];
            foreach (ICloneable item in e)
            {
                //var existingItem = AllItems.FirstOrDefault(s => s?.Id == item.Id, null);
                //if (existingItem != null)
                //{
                //    invalidItems.Add(item);
                //    continue;
                //}

                if (TransferingItemFilter != null && !TransferingItemFilter(item, AllItems))
                {
                    invalidItems.Add(item);
                    continue;
                }
                object cloningItem = item.Clone();
                AllItems.Add(cloningItem);
                cloningItems.Add(cloningItem);
            }
            RefreshItemCount();
            RefreshManagingItems();
            ItemsSelectionChanged?.Invoke(this, cloningItems.Cast<object>().ToList());

            if (invalidItems.Count != 0)
            {
                OnInvalidItemsTranferred?.Invoke(this, invalidItems);
            }
        }
        // ----------------------------------------------------------------------------------------
        // Edit Handling
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

            //var existingItem = AllItems.FirstOrDefault(s => s?.Id == e.oldItem.Id, null);
            //if (existingItem == null)
            //{
            //    return;
            //}
            var existingItem = OldItemValidationOnEditing?.Invoke(e.oldItem, AllItems);
            if (existingItem == null)
            {
                return;
            }

            //if (e.newItem.BirthDate.Date == DateTimeToStringConverter.ERROR_DATETIME.Date)
            //{
            //    e.newItem.BirthDate = e.oldItem.BirthDate;
            //}
            ItemTransformerOnEditting?.Invoke(e.oldItem, ref e.newItem);


            // existingItem.Copy(e.newItem);
            ExistingItemTransformerOnEditting?.Invoke(e.newItem, ref existingItem);
            RefreshManagingItems();
        }
        // ----------------------------------------------------------------------------------------
        // Item Removal Handling

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
        // ----------------------------------------------------------------------------------------
        // Item Creation Handling
        public void HandleItemsCreation(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                return;
            }

            var emptyItem = EmptyItem();
            if (emptyItem is null)
            {
                return;
            }

            // emptyItem.RevalidateAllProperties();
            if (emptyItem is INotifyDataErrorInfoExtended errorNotifiable) {
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

        // ----------------------------------------------------------------------------------------
        // Item Update Handling
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

                //if (updatingItem.Id == -1)
                //{
                //    validAddingItems.Add(updatingItem);
                //}
                //else
                //{
                //    validUpdatingItems.Add(updatingItem);
                //}
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

            //invalidItemsInfo.AddRange(
            //    validAddingItems
            //        .Where(x => addedItems.Where(a => a.Equals(x)).Any())
            //        .Select(x => {
            //            (object item, IEnumerable<String> errors) = (x, ["Added failed in database"]);
            //            return (item, errors);
            //            })
            //    );
            invalidItemsInfo.AddRange(invalidAddingItems);
            invalidItemsInfo.AddRange(invalidUpdatingItems);

            var invalidItems = invalidItemsInfo.Select(x => x.item);
            var modifiedItems = e.Where(x => !invalidItems.Contains(x)).ToList() ?? [];
            OnItemsUpdated?.Invoke(this, (modifiedItems, invalidItemsInfo));
        }
        // ----------------------------------------------------------------------------------------
        // Item Deletion Handling
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

            //for (int i = AllItems.Count - 1; i >= 0; i--)
            //{
            //    var currentItem = AllItems[i];
            //    if (deletedItems.Contains(currentItem))
            //    {
            //        AllItems.RemoveAt(i);
            //    }
            //}
            //RefreshItemCount();
            //RefreshManagingItems();
            OnItemsUpdated?.Invoke(this, (deletedItems, invalidItemsInfo));
        }

        // ----------------------------------------------------------------------------------------
        // Error Handling
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
