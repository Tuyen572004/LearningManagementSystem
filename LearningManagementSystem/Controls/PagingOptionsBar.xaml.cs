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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Controls
{
    public interface IPagingProvider: INotifyPropertyChanged
    {
        public int PageCount { get; }
        public void NavigateToPage(int page);
    }
    public partial class UnassignedPagingProvider : IPagingProvider
    {
        public int PageCount => 0;

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
            }
        }

        private void OnContextProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPagingProvider.PageCount))
            {
                // Call the desired function when PageCount changes
                CreatePageButtons();
            }
        }

        public IPagingProvider ContextProvider
        {
            get => (IPagingProvider)GetValue(ContextProviderProperty);
            set {
                SetValue(ContextProviderProperty, value);
                CreatePageButtons();
            }
        }

        private int CurrentPage { get; set; } = 1;
        public int PageWindowSize { get; set; } = 1;
        public PagingOptionsBar()
        {
            this.InitializeComponent();
        }

        public void ResetCurrentPage(bool preservedIfAble = true)
        {
            if (!preservedIfAble || CurrentPage > ContextProvider.PageCount)
            {
                CurrentPage = 1;
            }
        }

        private void CreatePageButtons()
        {
            ButtonContainer.Children.Clear();
            ResetCurrentPage();
            int pageCount = ContextProvider.PageCount;
            int leftBound = Math.Max(CurrentPage - PageWindowSize, 1);
            int rightBound = Math.Min(CurrentPage + PageWindowSize, pageCount);

            Button previousButton = new()
            {
                Content = "Previous",
                Tag = "Previous",
                Style = (Style)Resources["PagingButtonStyle"]
            };
            previousButton.Click += PreviousButton_Click;
            if (CurrentPage <= 1)
            {
                previousButton.IsEnabled = false;
            }
            else
            {
                previousButton.IsEnabled = true;
            }
            ButtonContainer.Children.Add(previousButton);

            Button nextButton = new()
            {
                Content = "Next",
                Tag = "Next",
                Style = (Style)Resources["PagingButtonStyle"]
            };
            nextButton.Click += NextButton_Click;
            if (CurrentPage >= pageCount)
            {
                nextButton.IsEnabled = false;
            }
            else
            {
                nextButton.IsEnabled = true;
            }
            ButtonContainer.Children.Add(nextButton);

            if (leftBound > 1)
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

            for (int i = leftBound; i <= rightBound; i++)
            {
                Button button = new()
                {
                    Content = i,
                    Tag = i,
                    Style = (Style)Resources["PagingButtonStyle"]
                };
                button.Click += Button_Click;
                ButtonContainer.Children.Add(button);
            }

            if (rightBound < pageCount)
            {
                if (rightBound < pageCount - 1)
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
                    Content = pageCount,
                    Tag = pageCount,
                    Style = (Style)Resources["PagingButtonStyle"]
                };
                lastButton.Click += Button_Click;
                ButtonContainer.Children.Add(lastButton);
            }

        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage += 1;
            ContextProvider.NavigateToPage(CurrentPage);
            
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage -= 1;
            ContextProvider.NavigateToPage(CurrentPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                CurrentPage = (int)button.Tag;
                ContextProvider.NavigateToPage(CurrentPage);
            }
        }
    }
}
