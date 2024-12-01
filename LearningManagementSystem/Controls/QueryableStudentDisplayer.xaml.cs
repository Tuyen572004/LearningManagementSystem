using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Controls
{
    // public interface IQueryableProvider: IStudentProvider, ISearchProvider, IPagingProvider;
    public sealed partial class QueryableStudentDisplayer : UserControl, IDisposable
    {
        private bool disposedValue;

        public SearchBar SearchBar => QSDSearchBar;
        public PagingOptionsBar PagingOptionsBar => QSDPagingOptionsBar;
        public StudentTable StudentTable => QSDStudentTable;
        public QueryableStudentDisplayer()
        {
            this.InitializeComponent();

            DataContextChanged += QueryableStudentDisplayer_DataContextChanged;
        }

        private void QueryableStudentDisplayer_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Should manually unsubscribe previous DataContext events

            // Subscribe to new DataContext events
            if (args.NewValue is ISearchProvider newSearchProvider)
            {
                SearchBar.SearchChanged += newSearchProvider.SearchChangedHandler;
                SetBinding(SearchBar, SearchBar.ContextProviderProperty);
                SearchBar.Visibility = Visibility.Visible;
            }
            if (args.NewValue is IStudentProvider newStudentProvider)
            {
                StudentTable.SortChanged += newStudentProvider.SortChangedHandler;
                StudentTable.StudentDoubleTapped += newStudentProvider.DoubleTappedHandler;
                
                
            }
            if (args.NewValue is IPagingProvider)
            {
                SetBinding(PagingOptionsBar, PagingOptionsBar.ContextProviderProperty);
                PagingOptionsBar.Visibility = Visibility.Visible;
            }
        }

        private void SetBinding(DependencyObject target, DependencyProperty property)
        {
            // Create a new Binding
            var binding = new Binding
            {
                Path = new PropertyPath(string.Empty),  // Bind to the current DataContext
                Source = DataContext,         // Set the binding source
                Mode = BindingMode.OneWay     // Use OneWay binding mode
            };

            // Apply the binding to the target property
            BindingOperations.SetBinding(target, property, binding);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Recommendations: dispose managed state (managed objects)

                    // Unsubscribe from DataContext events
                    if (DataContext is ISearchProvider searchProvider)
                    {
                        SearchBar.SearchChanged -= searchProvider.SearchChangedHandler;
                    }
                    if (DataContext is IStudentProvider studentProvider)
                    {
                        StudentTable.SortChanged -= studentProvider.SortChangedHandler;
                        StudentTable.StudentDoubleTapped -= studentProvider.DoubleTappedHandler;
                    }
                }

                // Recommendations: set large fields to null
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
