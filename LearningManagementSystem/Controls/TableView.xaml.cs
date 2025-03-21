#nullable enable
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Controls
{
    /// <summary>
    /// Specifies the severity level of an InfoBarMessage.
    /// </summary>
    public enum InfoBarMessageSeverity
    {
        Info,
        Warning,
        Error
    }
    /// <summary>
    /// Represents a message to be displayed in an InfoBar.
    /// </summary>
    public class InfoBarMessage
    {
        /// <summary>
        /// Gets or sets the severity level of the message.
        /// </summary>
        public InfoBarMessageSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the title of the message.
        /// </summary>
        required public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        required public string Message { get; set; }
    }
    /// <summary>
    /// Provides a method to validate a property.
    /// </summary>
    public interface IValidatableItem
    {
        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        void ValidateProperty(string? propertyName);
    }
    /// <summary>
    /// Provides a method to get an InfoBarMessage for a specific item.
    /// </summary>
    public interface IInfoProvider
    {
        /// <summary>
        /// Gets the InfoBarMessage for the specified item.
        /// </summary>
        /// <param name="item">The item to get the message for.</param>
        /// <returns>The InfoBarMessage for the item.</returns>
        public InfoBarMessage? GetMessageOf(object item);
    }
    /// <summary>
    /// Represents the criteria for sorting a column in a DataGrid.
    /// </summary>
    public class SortCriteria
    {
        /// <summary>
        /// Gets or sets the tag of the column to sort.
        /// </summary>
        required public string ColumnTag { get; set; }

        /// <summary>
        /// Gets or sets the sort direction for the column.
        /// </summary>
        public DataGridSortDirection? SortDirection { get; set; }
    }
    /// <summary>
    /// Provides data and event handlers for managing items in a table.
    /// </summary>
    public interface ITableItemProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the collection of items being managed.
        /// </summary>
        public ObservableCollection<object> ManagingItems { get; }

        /// <summary>
        /// Gets the event handler for when the sort criteria changes.
        /// </summary>
        public EventHandler<List<SortCriteria>>? SortChangedHandler => null;

        /// <summary>
        /// Gets the event handler for when an item is double-tapped.
        /// </summary>
        public EventHandler<object>? ItemDoubleTappedHandler => null;

        /// <summary>
        /// Gets the event handler for when an item is edited.
        /// The item should implement the ICloneable interface. If not, the oldItem will be the same as newItem.
        /// </summary>
        public EventHandler<(object oldItem, object newItem)>? ItemEdittedHandler => null;

        /// <summary>
        /// Gets the collection of column names to ignore.
        /// </summary>
        public IEnumerable<string> IgnoringColumns => [];

        /// <summary>
        /// Gets the collection of column names in the desired order.
        /// </summary>
        public IEnumerable<string> ColumnOrder => [];

        /// <summary>
        /// Gets the collection of column converters.
        /// </summary>
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];

        /// <summary>
        /// Gets the collection of read-only column names.
        /// </summary>
        public IEnumerable<string> ReadOnlyColumns => [];
    }
    public partial class UnassignedItemProvider : ITableItemProvider
    {
        public ObservableCollection<object> ManagingItems { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public sealed partial class TableView : UserControl, IDisposable
    {
        public static readonly DependencyProperty ContextProviderProperty =
            DependencyProperty.Register(
                "ContextProvider",
                typeof(ITableItemProvider),
                typeof(TableView),
                new PropertyMetadata(new UnassignedItemProvider(), OnDataChanged)
                );

        public ITableItemProvider ContextProvider
        {
            get => (ITableItemProvider)GetValue(ContextProviderProperty);
            set => SetValue(ContextProviderProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Do nothing currently
            if (d is TableView control)
            {
                if (e.NewValue is IInfoProvider)
                {
                    control.SeeItemInfoFlyout.Visibility = Visibility.Visible;
                }
                else
                {
                    control.SeeItemInfoFlyout.Visibility = Visibility.Collapsed;
                }

                if (e.OldValue is ITableItemProvider oldItemProvider)
                {
                    control.SortChanged -= oldItemProvider.SortChangedHandler;
                    control.ItemDoubleTapped -= oldItemProvider.ItemDoubleTappedHandler;
                    control.ItemEditted -= oldItemProvider.ItemEdittedHandler;
                }
                if (e.NewValue is ITableItemProvider newItemProvider)
                {
                    control.SortChanged += newItemProvider.SortChangedHandler;
                    control.ItemDoubleTapped += newItemProvider.ItemDoubleTappedHandler;
                    control.ItemEditted += newItemProvider.ItemEdittedHandler;
                }
            }
        }

        public TableView()
        {
            this.InitializeComponent();
        }

        public event EventHandler<List<SortCriteria>>? SortChanged;
        private readonly List<SortCriteria> _sortList = [];
        private readonly Dictionary<string, DataGridSortDirection?> _sortDirectionMapper = [];
        private void Dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var columnTag = e.Column.Tag?.ToString();
            if (columnTag is null)
            {
                return;
            }

            if (e.Column.SortDirection == null)
            {
                _sortDirectionMapper[columnTag] = DataGridSortDirection.Ascending;
            }
            else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                _sortDirectionMapper[columnTag] = DataGridSortDirection.Descending;
            }
            else
            {
                _sortDirectionMapper[columnTag] = null;
                _sortList.RemoveAll(s => s.ColumnTag == columnTag);
                SortChanged?.Invoke(this, _sortList);
                return;
            }

            var oldItem = _sortList.FirstOrDefault(s => s?.ColumnTag == columnTag, null);
            if (oldItem is not null)
            {
                oldItem.SortDirection = _sortDirectionMapper[columnTag];
            }
            else
            {
                _sortList.Add(new SortCriteria
                {
                    ColumnTag = columnTag,
                    SortDirection = _sortDirectionMapper[columnTag]
                });
            }

            SortChanged?.Invoke(this, _sortList);
        }

        public bool IsEditable
        {
            get => Dg.IsReadOnly;
            set => Dg.IsReadOnly = !value;
        }

        public event EventHandler<object>? ItemDoubleTapped;

        private void Dg_DoubleTapped(object _, DoubleTappedRoutedEventArgs __)
        {
            ItemDoubleTapped?.Invoke(this, Dg.SelectedItem);
        }

        public void RefreshData()
        {
            SortChanged?.Invoke(this, _sortList);
        }

        private void RefreshMenuLayout_Click(object _, RoutedEventArgs __)
        {
            RefreshData();
        }

        public event EventHandler<(object oldItem, object newItem)>? ItemEditted;

        // Don't delete this private variable
        // This code somehow runs Dg_CellEditEnding indefinitely if ran without the _isEdittingRow flag guard
        private bool _isEdittingRow = false;
        private object? _originalItem = null;
        private void Dg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _isEdittingRow = true;
            if (e.Row.DataContext is ICloneable cloneable)
            {
                _originalItem = cloneable.Clone();
            }
            else
            {
                _originalItem = e.Row.DataContext;
            }
        }
        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (!_isEdittingRow || _originalItem is null)
            //{
            //    return;
            //}
            //if (e.EditAction == DataGridEditAction.Commit)
            //{
            //    _isEdittingRow = false;
            //    ItemEditted?.Invoke(this, (_originalItem, (object)e.Row.DataContext));
            //}
            //else if (e.EditAction == DataGridEditAction.Cancel)
            //{
            //    _isEdittingRow = false;
            //    _originalItem = null;
            //}
            //return;
        }

        private void Dg_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            if (!_isEdittingRow || _originalItem is null)
            {
                return;
            }
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // For some reasons, when the event in ItemEditted is called and would modify the TableView rows,
                // (in this case, de-select the current editing cell's row), the Dg_CellEditEnded event raises an Exception.
                
                // Solution: Using DispatcherQueue
                
                // Ensures that the ItemEditted event is triggered
                // after the behind-the-scenes code in Dg_CellEditEnded has finished.

                var oldItem = _originalItem;
                DispatcherQueue.TryEnqueue(() =>
                {
                    ItemEditted?.Invoke(this, (oldItem, e.Row.DataContext));
                    if (e.Row.DataContext is IValidatableItem validatable)
                    {
                        validatable.ValidateProperty(e.Column.Tag?.ToString());

                        _lastSeenItem = e.Row.DataContext;
                        ResetItemInfo(forcedOpen: true);
                    }
                });
                // ItemEditted?.Invoke(this, (_originalItem, (object)e.Row.DataContext));
            }
            _isEdittingRow = false;
            _originalItem = null;
        }

        public IList<object> GetSelectedItems()
        {
            return Dg.SelectedItems.Cast<object>().ToList();
        }
        private void HandleItemsReselection(object? sender, IList<object> e)
        {
            if (sender is null)
            {
                return;
            }
            Dg.SelectedItems.Clear();
            foreach (var tableItem in Dg.ItemsSource)
            {
                if (e.Any(observingItem => ReferenceEquals(tableItem, observingItem)))
                {
                    Dg.SelectedItems.Add(tableItem);
                }
            }
        }
        public EventHandler<IList<object>> ItemsReselectionHandler => HandleItemsReselection;

        private object? _lastSeenItem = null;
        private void SeeItemInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            if (Dg.SelectedItem is not null)
            {
                _lastSeenItem = Dg.SelectedItem;
            }
            ResetItemInfo();
        }
        private void ResetItemInfo(bool forcedOpen = false)
        {
            if (ContextProvider is IInfoProvider infoProvider && _lastSeenItem is not null)
            // if (DataContext is IInfoProvider infoProvider && Dg.SelectedItem is not null)
            {
                var message = infoProvider.GetMessageOf(_lastSeenItem);
                if (message is not null)
                {
                    Dg_InfoBar.Title = message.Title;
                    Dg_InfoBar.Message = message.Message;
                    Dg_InfoBar.Severity = message.Severity switch
                    {
                        InfoBarMessageSeverity.Info => InfoBarSeverity.Informational,
                        InfoBarMessageSeverity.Warning => InfoBarSeverity.Warning,
                        InfoBarMessageSeverity.Error => InfoBarSeverity.Error,
                        _ => InfoBarSeverity.Informational
                    };
                    Dg_InfoBar.IsOpen = true;
                    return;
                }
                // The message is null, so close the InfoBar, regardless of the forcedOpen flag
                forcedOpen = false;
            }

            Dg_InfoBar.IsOpen = forcedOpen || false;
            _lastSeenItem = (Dg_InfoBar.IsOpen) ? _lastSeenItem : null;
        }

        public void ShowInfoBar(InfoBarMessage message)
        {
            Dg_InfoBar.Title = message.Title;
            Dg_InfoBar.Message = message.Message;
            Dg_InfoBar.Severity = message.Severity switch
            {
                InfoBarMessageSeverity.Info => InfoBarSeverity.Informational,
                InfoBarMessageSeverity.Warning => InfoBarSeverity.Warning,
                InfoBarMessageSeverity.Error => InfoBarSeverity.Error,
                _ => InfoBarSeverity.Informational
            };
            Dg_InfoBar.IsOpen = true;
        }

        private Dictionary<string, IValueConverter>? _converterMapper = null; 
        private Dictionary<string, int> _displayIndexMapper = [];

        private bool _isReorderingColumns = false;
        private readonly List<DataGridColumn> _generatingColumns = [];
        private ObservableCollection<DataGridColumn>? _actualColumnsReference = null;
        private void OnGeneratedColumns()
        {
            _isReorderingColumns = false;
            if (_actualColumnsReference is not null)
            {
                var temporaryColumn = _actualColumnsReference.FirstOrDefault(c => c.Tag?.ToString() == temporaryColumnTag);
                if (temporaryColumn is not null)
                {
                    _actualColumnsReference.Remove(temporaryColumn);
                }
                _actualColumnsReference = null;
            }
            _generatingColumns.Clear();
            _displayIndexMapper.Remove(temporaryColumnTag);
        }
        static private void AdjustColumnsOrdering(
            DataGridColumn currentColumn, // C++ ref
            List<DataGridColumn> generatedColumns, // C++ ref
            Dictionary<string, int> referenceIndexMapper // C++ ref
        )
        {
            var generatedColumnsSize = generatedColumns.Count;
            var currentColumnTag = currentColumn.Tag?.ToString() ?? $"ColumnG{generatedColumnsSize}";
            if (!referenceIndexMapper.TryGetValue(currentColumnTag, out int _))
            {
                currentColumn.DisplayIndex = generatedColumnsSize - 1;
                referenceIndexMapper.Add(currentColumnTag, generatedColumnsSize - 1);
                if (referenceIndexMapper.TryGetValue(temporaryColumnTag, out int _)) {
                    referenceIndexMapper[temporaryColumnTag] = generatedColumnsSize - 1;
                }
                generatedColumns.Add(currentColumn);
                return;
            }
            int nextIndex = 0;
            generatedColumns.Add(currentColumn);
            referenceIndexMapper
                .Where(pair => generatedColumns.Any(c => c.Tag?.ToString() == pair.Key))
                .OrderBy(pair => pair.Key == temporaryColumnTag ? referenceIndexMapper.Count - 1 : pair.Value)
                .ToList()
                .ForEach(pair =>
                {
                    DataGridColumn column = generatedColumns.First(c => c.Tag.ToString() == pair.Key);
                    if (column.Tag.ToString() == temporaryColumnTag)
                    {
                        // -1 due to "Count" and "Index" offset
                        // -1 so that both the temporary column and the current column should have the same DisplayIndex
                        // (why the same display index: so that the new column would be placed before the temporary column)
                        column.DisplayIndex = generatedColumns.Count - 2;
                        referenceIndexMapper[temporaryColumnTag] = generatedColumns.Count - 2;
                    }
                    else
                    {
                        column.DisplayIndex = nextIndex++;
                    }
                });
        }
        const string temporaryColumnTag = "TO_BE_DELETED_UwU";
        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (ContextProvider is ITableItemProvider provider)
            {
                var ignoringColumns = provider.IgnoringColumns.ToList();
                if (ignoringColumns.Contains(e.PropertyName))
                {
                    e.Cancel = true;
                    return;
                }
                if (provider.ReadOnlyColumns.Contains(e.PropertyName))
                {
                    e.Column.IsReadOnly = true;
                }
                _converterMapper ??= provider.ColumnConverters.ToDictionary(c => c.ColumnName, c => c.Converter);
            }
            e.Column.Tag = e.PropertyName;

            if (!_isReorderingColumns)
            {
                bool hasRememberedColumnOrderAtFirstLoad = _generatingColumns.Count != 0;
                DispatcherQueue.TryEnqueue(() => {
                    if (!hasRememberedColumnOrderAtFirstLoad)
                    {
                        RememberColumnOrderAtFirstLoad();
                    }
                    OnGeneratedColumns();
                });
                if (sender is not null)
                {
                    // The temporary column is needed to allow assigning "overflow"-ing DisplayIndex
                    // in the AdjustColumnsOrdering method, as the current column is not yet "acknowledge"-ed by the DataGrid
                    var temporaryColumn = new DataGridTextColumn()
                    {
                        Tag = temporaryColumnTag
                    };
                    (sender as DataGrid)?.Columns.Add(temporaryColumn);
                    _generatingColumns.Add(temporaryColumn);
                    _displayIndexMapper.Add(temporaryColumnTag, 1);
                    _actualColumnsReference = (sender as DataGrid)?.Columns;

                }
                DispatcherQueue.TryEnqueue(OnGeneratedColumns);
                _isReorderingColumns = true;
            }

            //// Set up column order for this colomn
            //if (_displayIndexMapper.TryGetValue(e.PropertyName, out int value))
            //{
            //    e.Column.DisplayIndex = value;
            //}
            //else
            //{
            //    e.Column.DisplayIndex = _columnIndexCounter++;
            //    _displayIndexMapper.Add(e.PropertyName, e.Column.DisplayIndex);
            //}
            AdjustColumnsOrdering(e.Column, _generatingColumns, _displayIndexMapper);

            // Set up converter for this column value
            if (_converterMapper is not null && _converterMapper.TryGetValue(e.PropertyName, out IValueConverter? converter))
            {
                if (converter is null)
                {
                    return;
                }
                var convertedColumn = e.Column as DataGridTextColumn;
                if (convertedColumn is not null)
                {
                    convertedColumn.Binding.Converter = converter;
                }
            }

            // Set up the sort status of this column
            e.Column.SortDirection = _sortDirectionMapper.GetValueOrDefault(e.PropertyName, null);
        }

        private void RememberColumnOrderAtFirstLoad()
        {
            if (ContextProvider is ITableItemProvider provider)
            {
                Dictionary<string, int> columnDisplayIndex = [];
                int columnIndexCounter = 0;
                List<string> existingColumnNames = [.. _displayIndexMapper.Keys];
                foreach (string columnName in provider.ColumnOrder)
                {
                    if (existingColumnNames.Contains(columnName))
                    {
                        columnDisplayIndex.Add(columnName, columnIndexCounter++);
                    }
                }
                foreach (string existingColumn in existingColumnNames)
                {
                    if (!columnDisplayIndex.ContainsKey(existingColumn))
                    {
                        if (existingColumn != temporaryColumnTag)
                        {
                            columnDisplayIndex.Add(existingColumn, columnIndexCounter++);
                        }
                    }
                }
                // In case the initial data is empty
                if (existingColumnNames.Count != 0)
                {
                    columnDisplayIndex.Add(temporaryColumnTag, columnIndexCounter);
                }
                _displayIndexMapper = columnDisplayIndex;
                //// Note: No need to re-order columns, as the columns should be already ordered at this time
                // foreach (var indexedTag in _displayIndexMapper.OrderBy(pair => pair.Value)) {

                foreach (var indexedTag in _displayIndexMapper) {
                    var column = Dg.Columns.FirstOrDefault(c => c.Tag?.ToString() == indexedTag.Key);
                    if (column is not null)
                    {
                        column.DisplayIndex = indexedTag.Value;
                    }
                }
            }
        }

        private void SelectedAllFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            Dg.SelectedItems.Clear();
            foreach (var item in Dg.ItemsSource)
            {
                Dg.SelectedItems.Add(item);
            }
        }


        private void Dg_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            _displayIndexMapper.Clear();
            foreach (DataGridColumn column in Dg.Columns)
            {
                _displayIndexMapper.Add(
                    column.Tag?.ToString() ?? $"ColumnCR{column.DisplayIndex}",
                    column.DisplayIndex
                    );
            }
        }

        private bool disposedValue;
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SortChanged -= ContextProvider.SortChangedHandler;
                    ItemDoubleTapped -= ContextProvider.ItemDoubleTappedHandler;
                    ItemEditted -= ContextProvider.ItemEdittedHandler;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
