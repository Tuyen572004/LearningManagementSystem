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
    }
    public partial class UnassignedStudentProvider : IStudentProvider
    {
        public ObservableCollection<StudentVer2> ManagingStudents { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
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

        public event EventHandler<List<SortCriteria>> SortChanged;
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
            _sortList.Add(new SortCriteria
            {
                ColumnTag = e.Column.Tag.ToString(),
                SortDirection = e.Column.SortDirection
            });

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

        public event EventHandler<List<StudentVer2>> StudentSelected;
        private void Dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                StudentSelected?.Invoke(this, e.AddedItems as List<StudentVer2>);
            }
        }

        public event EventHandler<StudentVer2> StudentDoubleTapped;
        private void Dg_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (Dg.SelectedItem is StudentVer2 student)
            {
                StudentDoubleTapped?.Invoke(this, student);
            }
        }
    }
}
