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
    public enum InfoBarMessageSeverity
    {
        Info,
        Warning,
        Error
    }
    public class InfoBarMessage
    {
        public InfoBarMessageSeverity Severity { get; set; }
        required public string Title { get; set; }
        required public string Message { get; set; }
    }
    public interface IValidatableItem
    {
        void ValidateProperty(string? propertyName);
    }
    public interface IInfoProvider
    {
        public InfoBarMessage? GetMessageOf(object item);
    }
    public class SortCriteria
    {
        required public string ColumnTag { get; set; }
        public DataGridSortDirection? SortDirection { get; set; }
    }
    public interface ITableItemProvider: INotifyPropertyChanged
    {
        public ObservableCollection<object> ManagingItems { get; }
        public EventHandler<List<SortCriteria>>? SortChangedHandler => null;
        public EventHandler<object>? ItemDoubleTappedHandler => null;

        /// <summary>
        /// The item should be implemented with ICloneable interface. If not, the oldItem will be the same as newItem.
        /// </summary>
        public EventHandler<(object oldItem, object newItem)>? ItemEdittedHandler => null;

        public IEnumerable<string> IgnoringColumns => [];
        public IEnumerable<string> ColumnOrder => [];
        public IEnumerable<(string ColumnName, IValueConverter Converter)> ColumnConverters => [];
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
                currentColumn.DisplayIndex = generatedColumnsSize;
                generatedColumns.Add(currentColumn);
                referenceIndexMapper.Add(currentColumnTag, generatedColumnsSize);
                return;
            }
            int nextIndex = 0;
            generatedColumns.Add(currentColumn);
            referenceIndexMapper
                .Where(pair => generatedColumns.Any(c => c.Tag?.ToString() == pair.Key))
                .OrderBy(pair => pair.Value)
                .ToList()
                .ForEach(pair =>
                {
                    DataGridColumn column = generatedColumns.First(c => c.Tag.ToString() == pair.Key);
                    column.DisplayIndex = nextIndex++;
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

            if (!_isReorderingColumns)
            {
                if (sender is not null)
                {
                    // The temporary column is needed to allow assigning "overflow"-ing DisplayIndex
                    // in the AdjustColumnsOrdering method, as the current column is not yet "acknowledge"-ed by the DataGrid
                    (sender as DataGrid)?.Columns.Add(new DataGridTextColumn()
                    {
                        Tag = temporaryColumnTag
                    });
                    _actualColumnsReference = (sender as DataGrid)?.Columns;

                }
                DispatcherQueue.TryEnqueue(OnGeneratedColumns);
                _isReorderingColumns = true;
            }
        }

        private void Dg_Loaded(object sender, RoutedEventArgs e)
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
                        columnDisplayIndex.Add(existingColumn, columnIndexCounter++);
                    }
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
