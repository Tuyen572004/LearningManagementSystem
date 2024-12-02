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
        }

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

        private void RefreshMenuLayout_Click(object sender, RoutedEventArgs e)
        {
            SortChanged?.Invoke(this, _sortList);
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
    }
}
