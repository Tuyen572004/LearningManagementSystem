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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Controls
{
    public interface IPagingProvider: INotifyPropertyChanged
    {
        public int CurrentPage { get; }
        public int ItemCount { get; }
        public int RowsPerPage { get; set; }
        public void NavigateToPage(int page);
    }
    public partial class UnassignedPagingProvider : IPagingProvider
    {
        public int CurrentPage => 1;
        public int ItemCount => 0;
        public int RowsPerPage { get; set; } = 10;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NavigateToPage(int page)
        {
            // Do nothing
        }
    }
    public sealed partial class PagingOptionsBar : UserControl
    {
        public static readonly DependencyProperty ContextProviderProperty =
            DependencyProperty.Register(
                "ContextProvider",
                typeof(IPagingProvider),
                typeof(PagingOptionsBar),
                new PropertyMetadata(new UnassignedPagingProvider(), OnDataChanged)
                );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PagingOptionsBar control)
            {
                if (e.OldValue is IPagingProvider oldProvider)
                {
                    oldProvider.PropertyChanged -= control.OnContextProviderPropertyChanged;
                }

                if (e.NewValue is IPagingProvider newProvider)
                {
                    newProvider.PropertyChanged += control.OnContextProviderPropertyChanged;
                }

                control.RefreshPaging();
                
            }
        }

        private void OnContextProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPagingProvider.ItemCount)
                || e.PropertyName == nameof(IPagingProvider.CurrentPage)
                || e.PropertyName == nameof(IPagingProvider.RowsPerPage))
            {
                // Call the desired function when PageCount changes
                RefreshPaging();
            }
        }

        public IPagingProvider ContextProvider
        {
            get => (IPagingProvider)GetValue(ContextProviderProperty);
            set {
                SetValue(ContextProviderProperty, value);
                RefreshPaging();
            }
        }

        public int CurrentPage { get => ContextProvider.CurrentPage; }
        public int ItemCount { get => ContextProvider.ItemCount; }
        public int PageCount => ItemCount / ContextProvider.RowsPerPage + ((ItemCount % ContextProvider.RowsPerPage > 0) ? 1 : 0);
        public int PageWindowSize { get; set; } = 1;
        private int FirstItemIndexInPage => (CurrentPage - 1) * ContextProvider.RowsPerPage + 1;
        public string PageCountText =>
            $"" +
            ((FirstItemIndexInPage < ItemCount) ? $"{FirstItemIndexInPage} - " : "") +
            $"{Math.Min(ItemCount, CurrentPage * ContextProvider.RowsPerPage)} (of total {ItemCount}),";
        public readonly List<int> PageSizes = [5, 10, 20, 50, 100];
        public PagingOptionsBar()
        {
            this.InitializeComponent();

            RefreshPaging();
        }

        public void RefreshPaging()
        {
            CreatePageButtons();
            PagingInfo.Text = PageCountText;
            RowSizeSelector.SelectedItem = null;
            RowSizeSelector.PlaceholderText = ContextProvider.RowsPerPage.ToString();
        }
        private void CreatePageButtons()
        {
            // This function was meant to created the paging buttons
            // even if the current page exceeds the page count.

            // So some conditions may be redundant or weird.

            ButtonContainer.Children.Clear();
            int leftBound = Math.Max(CurrentPage - PageWindowSize, 1);
            // int rightBound = Math.Min(CurrentPage + PageWindowSize, pageCount);
            int rightBound = CurrentPage + PageWindowSize;

            Button previousButton = new()
            {
                Content = "Previous",
                Tag = "Previous",
                Style = (Style)Resources["PagingButtonStyle"]
            };     
            if (CurrentPage <= 1 || PageCount <= 0 || CurrentPage > PageCount + 1)
            {
                previousButton.IsEnabled = false;
            }
            else
            {
                previousButton.IsEnabled = true;
                previousButton.Click += PreviousButton_Click;
            }
            ButtonContainer.Children.Add(previousButton);

            Button nextButton = new()
            {
                Content = "Next",
                Tag = "Next",
                Style = (Style)Resources["PagingButtonStyle"]
            };  
            if (CurrentPage >= PageCount)
            {
                nextButton.IsEnabled = false;
            }
            else
            {
                nextButton.IsEnabled = true;
                nextButton.Click += NextButton_Click;
            }
            ButtonContainer.Children.Add(nextButton);

            if (leftBound > 1 && PageCount > 0)
            {
                Button firstButton = new()
                {
                    Content = 1,
                    Tag = 1,
                    Style = (Style)Resources["PagingButtonStyle"]
                };
                firstButton.Click += Button_Click;
                ButtonContainer.Children.Add(firstButton);

                if (leftBound > 2)
                {
                    TextBlock ellipsisBlock = new()
                    {
                        Text = "...",
                        Tag = leftBound - 1,
                        Style = (Style)Resources["EllipsisTextBlockStyle"]
                    };
                    ButtonContainer.Children.Add(ellipsisBlock);
                }
            }

            for (int i = leftBound; i <= rightBound && i <= PageCount; i++)
            {
                Button button = new()
                {
                    Content = i,
                    Tag = i,
                    Style = (Style)Resources["PagingButtonStyle"]
                };
                if (i == CurrentPage)
                {
                    button.IsEnabled = false;
                }
                button.Click += Button_Click;
                ButtonContainer.Children.Add(button);
            }

            if ((rightBound < PageCount) || (CurrentPage > PageCount && PageCount > 0))
            {
                if (rightBound < PageCount - 1)
                {
                    TextBlock ellipsisBlock = new()
                    {
                        Text = "...",
                        Tag = rightBound + 1,
                        Style = (Style)Resources["EllipsisTextBlockStyle"]
                    };
                    ButtonContainer.Children.Add(ellipsisBlock);
                }

                Button lastButton = new()
                {
                    Content = PageCount,
                    Tag = PageCount,
                    Style = (Style)Resources["PagingButtonStyle"]
                };
                lastButton.Click += Button_Click;
                ButtonContainer.Children.Add(lastButton);
            }

            if (CurrentPage > PageCount && PageCount > 0)
            {
                if (CurrentPage > PageCount + 1)
                {
                    TextBlock ellipsisBlock = new()
                    {
                        Text = "...",
                        Tag = PageCount + 1,
                        Style = (Style)Resources["EllipsisTextBlockStyle"]
                    };
                    ButtonContainer.Children.Add(ellipsisBlock);
                }

                Button outOfBoundButton = new()
                {
                    Content = CurrentPage,
                    Tag = CurrentPage,
                    Style = (Style)Resources["PagingButtonStyle"],
                    IsEnabled = false
                };
                outOfBoundButton.Click += Button_Click;
                ButtonContainer.Children.Add(outOfBoundButton);
            }

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ContextProvider.NavigateToPage(CurrentPage + 1);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ContextProvider.NavigateToPage(CurrentPage - 1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ContextProvider.NavigateToPage((int)button.Tag);
            }
        }

        private void RowSizeSelector_SelectionChanged(object sender, SelectionChangedEventArgs _)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem is int newSize)
                {
                    ContextProvider.RowsPerPage = newSize;
                    ContextProvider.NavigateToPage(1);
                }
            }
        }

        //private void PagingDetailEnabler_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (sender is ToggleButton toggleButton)
        //    {
        //        if (toggleButton.IsChecked == true)
        //        {
        //            PagingDetail.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            PagingDetail.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        //private void PagingDetailEnabler_Checked(object sender, RoutedEventArgs _)
        //{
        //    if (sender is ToggleButton)
        //    {
        //        PagingDetail.Visibility = Visibility.Visible;
        //    }
        //}

        //private void PagingDetailEnabler_Unchecked(object sender, RoutedEventArgs _)
        //{
        //    if (sender is ToggleButton)
        //    {
        //        PagingDetail.Visibility = Visibility.Collapsed;
        //    }
        //}
    }
}
