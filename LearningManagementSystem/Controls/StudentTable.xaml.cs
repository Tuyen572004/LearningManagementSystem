#nullable enable
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.UI.Controls;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
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
    internal class GeneratedColumnDetail
    {
        required public string Name { get; set; }
        required public DataGridColumn Column { get; set; }
    }
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
    public interface IInfoProvider
    {
        public InfoBarMessage? GetMessageOf(object item);
    }
    public class SortCriteria
    {
        required public string ColumnTag { get; set; }
        public DataGridSortDirection? SortDirection { get; set; }
    }
    public interface IStudentProvider: INotifyPropertyChanged
    {
        public ObservableCollection<StudentVer2> ManagingStudents { get; }
        public EventHandler<List<SortCriteria>>? SortChangedHandler => null;
        public EventHandler<StudentVer2>? StudentDoubleTappedHandler => null;
        public EventHandler<(StudentVer2 oldStudent, StudentVer2 newStudent)>? StudentEdittedHandler => null;

        public IEnumerable<String> IgnoringColumns => ["HasErrors"];
        public IEnumerable<String> ColumnOrder => ["Id", "UserId"];
    }
    public partial class UnassignedStudentProvider : IStudentProvider
    {
        public ObservableCollection<StudentVer2> ManagingStudents { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public sealed partial class StudentTable : UserControl
    {
        // private readonly IStudentProvider _dataContext = null;
        public static readonly DependencyProperty ContextProviderProperty =
            DependencyProperty.Register(
                "ContextProvider",
                typeof(IStudentProvider),
                typeof(StudentTable),
                new PropertyMetadata(new UnassignedStudentProvider(), OnDataChanged)
                );

        public IStudentProvider ContextProvider
        {
            get => (IStudentProvider)GetValue(ContextProviderProperty);
            set => SetValue(ContextProviderProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Do nothing currently
        }

        public StudentTable()
        {
            this.InitializeComponent();

            // DataContextChanged += StudentTable_DataContextChanged;
        }
        //private object? _oldDataContext = null;
        //private void StudentTable_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{
        //    var OldValue = _oldDataContext;
        //    if (OldValue is IStudentProvider oldStudentProvider)
        //    {
        //        SortChanged -= oldStudentProvider.SortChangedHandler;
        //        StudentDoubleTapped -= oldStudentProvider.StudentDoubleTappedHandler;
        //        StudentEditted -= oldStudentProvider.StudentEdittedHandler;
        //    }

        //    if (args.NewValue is IStudentProvider newStudentProvider)
        //    {
        //        SortChanged += newStudentProvider.SortChangedHandler;
        //        StudentDoubleTapped += newStudentProvider.StudentDoubleTappedHandler;
        //        StudentEditted += newStudentProvider.StudentEdittedHandler;
        //    }
        //    _oldDataContext = args.NewValue;
        //}

        public event EventHandler<List<SortCriteria>>? SortChanged;
        private readonly List<SortCriteria> _sortList = [];

        private void Dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.SortDirection == null)
            {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                e.Column.SortDirection = DataGridSortDirection.Descending;
            }
            else
            {
                _sortList.RemoveAll(s => s.ColumnTag == e.Column.Tag.ToString());
                e.Column.SortDirection = null;
                SortChanged?.Invoke(this, _sortList);
                return;
            }

            var oldItem = _sortList.FirstOrDefault(s => s?.ColumnTag == e.Column.Tag.ToString(), null);
            if (oldItem is not null)
            {
                oldItem.SortDirection = e.Column.SortDirection;
            }
#pragma warning disable CS8601 // Possible null reference assignment.
            _sortList.Add(new SortCriteria
            {
                ColumnTag = e.Column.Tag.ToString(),
                SortDirection = e.Column.SortDirection
            });
#pragma warning restore CS8601 // Possible null reference assignment.

            //SortChanged?.Invoke(this, new SortCriteria
            //{
            //    ColumnTag = e.Column.Tag.ToString(),
            //    SortDirection = e.Column.SortDirection
            //});

            SortChanged?.Invoke(this, _sortList);
        }

        public void SkipColumns(params string[] columnTags)
        {
            foreach (var tag in columnTags)
            {
                Dg.Columns.First(c => c.Tag.ToString() == tag).Visibility = Visibility.Collapsed;

            }
        }

        public void EnableColumns(params string[] columnTags)
        {
            foreach (var tag in columnTags)
            {
                Dg.Columns.First(c => c.Tag.ToString() == tag).Visibility = Visibility.Visible;

            }
        }

        public bool IsEditable
        {
            get => Dg.IsReadOnly;
            set => Dg.IsReadOnly = !value;
        }

        public event EventHandler<StudentVer2>? StudentDoubleTapped;
        private void Dg_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (Dg.SelectedItem is StudentVer2 student)
            {
                StudentDoubleTapped?.Invoke(this, student);
            }
        }

        public void RefreshData()
        {
            SortChanged?.Invoke(this, _sortList);
        }

        private void RefreshMenuLayout_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        public event EventHandler<(StudentVer2 oldStudent, StudentVer2 newStudent)>? StudentEditted;

        // Don't delete this private variable
        // This code somehow runs Dg_CellEditEnding indefinitely if ran without the _isEdittingRow flag guard
        private bool _isEdittingRow = false;
        private StudentVer2? _originalStudent = null;
        private void Dg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _isEdittingRow = true;
            _originalStudent = (StudentVer2)((ICloneable)e.Row.DataContext).Clone();
        }
        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (!_isEdittingRow || _originalStudent is null)
            //{
            //    return;
            //}
            //if (e.EditAction == DataGridEditAction.Commit)
            //{
            //    _isEdittingRow = false;
            //    StudentEditted?.Invoke(this, (_originalStudent, (StudentVer2)e.Row.DataContext));
            //}
            //else if (e.EditAction == DataGridEditAction.Cancel)
            //{
            //    _isEdittingRow = false;
            //    _originalStudent = null;
            //}
            //return;
        }

        private void Dg_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            if (!_isEdittingRow || _originalStudent is null)
            {
                return;
            }
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // For some reasons, when the event in StudentEditted is called and would modify the TableView rows,
                // (in this case, de-select the current editing cell's row), the Dg_CellEditEnded event raises an Exception.
                
                // Solution: Using DispatcherQueue
                
                // Ensures that the StudentEditted event is triggered
                // after the behind-the-scenes code in Dg_CellEditEnded has finished.

                var oldStudent = (StudentVer2)_originalStudent.Clone();
                DispatcherQueue.TryEnqueue(() =>
                {
                    StudentEditted?.Invoke(this, (oldStudent, (StudentVer2)e.Row.DataContext));
                    // TODO - Technical Debt: Ugly validation logic here, not generic friendly
                    ((StudentVer2)e.Row.DataContext)?.ValidateProperty(e.Column.Tag.ToString());
                });
                // StudentEditted?.Invoke(this, (_originalStudent, (StudentVer2)e.Row.DataContext));
            }
            _isEdittingRow = false;
            _originalStudent = null;
        }

        public IList<StudentVer2> GetSelectedItems()
        {
            return Dg.SelectedItems.Cast<StudentVer2>().ToList();
        }
        private void HandleItemsReselection(object? sender, IList<StudentVer2> e)
        {
            if (sender is null)
            {
                return;
            }
            Dg.SelectedItems.Clear();
            foreach (var studentInTable in Dg.ItemsSource)
            {
                if (e.Any(observingStudent => ReferenceEquals(studentInTable, observingStudent)))
                {
                    Dg.SelectedItems.Add(studentInTable);
                }
            }
        }
        public EventHandler<IList<StudentVer2>> ItemsReselectionHandler => HandleItemsReselection;

        private void SeeItemInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            if (ContextProvider is IInfoProvider infoProvider && Dg.SelectedItem is not null)
            // if (DataContext is IInfoProvider infoProvider && Dg.SelectedItem is not null)
            {
                var message = infoProvider.GetMessageOf(Dg.SelectedItem);
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
                }
            }
            else
            {
                Dg_InfoBar.IsOpen = false;
            }
        }

        private List<GeneratedColumnDetail> _generatedColumnsDetail = [];
        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (ContextProvider is IStudentProvider provider)
            {
                var ignoringColumns = provider.IgnoringColumns.ToList();
                if (ignoringColumns.Contains(e.PropertyName))
                {
                    e.Cancel = true;
                    return;
                }
            }
            _generatedColumnsDetail.Add(new() { Column = e.Column, Name = e.PropertyName });
        }

        private void Dg_Loaded(object sender, RoutedEventArgs e)
        {
            if (ContextProvider is IStudentProvider provider)
            {
                Dictionary<DataGridColumn, int> columnDisplayIndex = [];
                int columnIndexCounter = 0;
                foreach (String columnName in provider.ColumnOrder)
                {
                    GeneratedColumnDetail? foundColumn = _generatedColumnsDetail.Find(columnDetail => columnDetail.Name == columnName);
                    if (foundColumn != null)
                    {
                        columnDisplayIndex.Add(foundColumn.Column, columnIndexCounter++);
                    }
                }
                foreach (GeneratedColumnDetail column in _generatedColumnsDetail)
                {
                    if (columnDisplayIndex.TryGetValue(column.Column, out int value))
                    {
                        column.Column.DisplayIndex = value;
                    } else
                    {
                        column.Column.DisplayIndex = columnIndexCounter++;
                    }
                }
            }
        }
    }
}
