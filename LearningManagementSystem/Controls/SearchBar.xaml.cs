#nullable enable
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Controls
{
    /// <summary>
    /// Interface for search providers that supply search fields and handle search changes.
    /// </summary>
    public interface ISearchProvider
    {
        /// <summary>
        /// Gets the list of search fields.
        /// </summary>
        public List<string> SearchFields { get; }

        /// <summary>
        /// Event handler for search criteria changes.
        /// </summary>
        public EventHandler<SearchCriteria?>? SearchChangedHandler => null;
    }

    /// <summary>
    /// A default implementation of ISearchProvider with no search fields.
    /// </summary>
    public class UnassignedSearchProvider : ISearchProvider
    {
        /// <summary>
        /// Gets an empty list of search fields.
        /// </summary>
        public List<string> SearchFields { get => []; }
    }

    /// <summary>
    /// Represents the criteria for a search operation.
    /// </summary>
    public class SearchCriteria
    {
        /// <summary>
        /// Gets or sets the search field.
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Gets or sets the search pattern.
        /// </summary>
        public string? Pattern { get; set; }

        // public bool IsCaseSensitive { get; set; }
    }
    public sealed partial class SearchBar : UserControl, IDisposable
    {
        public static readonly DependencyProperty ContextProviderProperty =
            DependencyProperty.Register(
                "ContextProvider",
                typeof(ISearchProvider),
                typeof(SearchBar),
                new PropertyMetadata(new UnassignedSearchProvider(), OnDataChanged)
                );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchBar control)
            {
                if (e.OldValue is ISearchProvider oldSearchProvider)
                {
                    control.SearchChanged -= oldSearchProvider.SearchChangedHandler;
                }
                if (e.NewValue is ISearchProvider newSearchProvider)
                {
                    control.SearchChanged += newSearchProvider.SearchChangedHandler;
                }
            }
        }

        public ISearchProvider ContextProvider
        {
            get => (ISearchProvider)GetValue(ContextProviderProperty);
            set
            {
                SetValue(ContextProviderProperty, value);
            }
        }

        readonly public Dictionary<string, string> SearchPattern = new()
        {
            { "Contains", "like '%{0}%'" },
            { "Starts with", "like '{0}%'" },
            { "Ends with", "like '%{0}'" },
            { "Match with", "like '{0}'" },
            { "Is not null", "is not null" },
            { "Is null", "is null" }
        };
        private bool disposedValue;

        public List<string> SearchOptions { get; } 

        public SearchBar()
        {
            this.InitializeComponent();

            SearchOptions = new(SearchPattern.Keys);
        }

        private void ComboBox_SearchField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do nothing
        }

        private void ComboBox_SearchScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var selectedItemText = comboBox.SelectedItem?.ToString();
                if (selectedItemText == "Is not null" || selectedItemText == "Is null")
                {
                    TextBox_Keyword.Text = "";
                    TextBox_Keyword.IsEnabled = false;
                }
                else
                {
                    TextBox_Keyword.IsEnabled = true;
                }
            }
        }

        public event EventHandler<SearchCriteria?>? SearchChanged;

        private async void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            List<string> problems = [];
            if (ComboBox_SearchField.SelectedItem == null)
            {
                problems.Add("- Search field is not selected.");
            }
            if (ComboBox_SearchScheme.SelectedItem == null)
            {
                problems.Add("- Search scheme is not selected.");
            }
            if (TextBox_Keyword.Text.Trim().Length == 0 && // When the length of keyword is zero
                !(ComboBox_SearchScheme.SelectedItem != null // But not when the scheme is null-check
                && (ComboBox_SearchScheme.SelectedItem.ToString() == "Is null"
                || ComboBox_SearchScheme.SelectedItem.ToString() == "Is not null"))
                )
            {
                problems.Add("- Keyword is empty.");
            }

            if (problems.Count != 0)
            {
                var errorDialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Invalid Search Parameter(s)",
                    Content = string.Join("\n", problems),
                    DefaultButton = ContentDialogButton.Close,
                    CloseButtonText = "OK"
                };
                var _ = await errorDialog.ShowAsync();
                return;
            }

            var newSearchCriteria = new SearchCriteria
            {
                Field = ComboBox_SearchField.SelectedItem?.ToString(),
                Pattern = string.Format(SearchPattern[ComboBox_SearchScheme.SelectedItem?.ToString() ?? ""], TextBox_Keyword.Text),
                // IsCaseSensitive = CheckBox_CaseSensitive.IsChecked == true
            };

            SearchChanged?.Invoke(this, newSearchCriteria);
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Keyword.Text = string.Empty;
            SearchChanged?.Invoke(this, null);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SearchChanged -= ContextProvider.SearchChangedHandler;
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

        private void ShowSearchOptionCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                AdvancedSearchOptionsStackPanel.Visibility = Visibility.Visible;
                CheckboxContainer.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        private void ShowSearchOptionCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                AdvancedSearchOptionsStackPanel.Visibility = Visibility.Collapsed;
                CheckboxContainer.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }
    }
}
