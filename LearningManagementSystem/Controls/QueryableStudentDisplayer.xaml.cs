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
    public sealed partial class QueryableStudentDisplayer : UserControl
    {
        public SearchBar SearchBar => QSDSearchBar;
        public PagingOptionsBar PagingOptionsBar => QSDPagingOptionsBar;
        public StudentTable StudentTable => QSDStudentTable;
        public QueryableStudentDisplayer()
        {
            this.InitializeComponent();

            DataContextChanged += QueryableStudentDisplayer_DataContextChanged;
        }
        private object _oldDataContext = null;
        private void QueryableStudentDisplayer_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            // Should manually unsubscribe previous DataContext events

            // Unsubscribe from previous DataContext events
            var OldValue = _oldDataContext;
            if (OldValue is ISearchProvider)
            {
                ClearValue(SearchBar.ContextProviderProperty);
                SearchBar.Visibility = Visibility.Collapsed;
            }
            if (OldValue is IPagingProvider)
            {
                ClearValue(PagingOptionsBar.ContextProviderProperty);
                PagingOptionsBar.Visibility = Visibility.Collapsed;
            }


            // Subscribe to new DataContext events
            if (args.NewValue is ISearchProvider)
            {
                SetBinding(SearchBar, SearchBar.ContextProviderProperty);
                SearchBar.Visibility = Visibility.Visible;
            }
            if (args.NewValue is IStudentProvider)
            {
                SetBinding(StudentTable, StudentTable.ContextProviderProperty);
            }
            if (args.NewValue is IPagingProvider)
            {
                SetBinding(PagingOptionsBar, PagingOptionsBar.ContextProviderProperty);
                PagingOptionsBar.Visibility = Visibility.Visible;
            }

            _oldDataContext = args.NewValue;
        }

        private void SetBinding(DependencyObject target, DependencyProperty property)
        {
            // Create a new Binding
            var binding = new Binding
            {
                Path = new PropertyPath("DataContext"),  // Bind to the current DataContext
                Source = this,         // Set the binding source
                Mode = BindingMode.OneWay     // Use OneWay binding mode
            };

            // Apply the binding to the target property
            BindingOperations.SetBinding(target, property, binding);
        }
    }
}
