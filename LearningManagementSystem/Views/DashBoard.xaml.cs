using LearningManagementSystem.Views;
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

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashBoard : Window { 

         //public static INavigationService NavigationService { get; private set; }
        public DashBoard()
        {
            this.InitializeComponent();
            //NavigationService = new FrameNavigationService(this.ContentFrame);
            NavigateByTag("LearningManagementSystem.HomePage");

            // Remember to navigate back
            // NavigateByTag("LearningManagementSystem.HomePage");
            menu.SelectionChanged += menu_SelectionChanged;
        }

        private void menu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item &&
                item.Tag is string tag)
            {
            //     TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
            // };

            // if (args.SelectedItemContainer.Tag is string selectedItemTag)
            // {
            //     switch (selectedItemTag)
            //     {
            //         case "home":
            //             ContentFrame.NavigateToType(typeof(HomePage), null, navOptions);
            //             break;
            //         case "courses":
            //             ContentFrame.NavigateToType(typeof(EnrollmentClassesPage), null, navOptions);
            //             break;
            //             //case "Students":
            //             //    ContentFrame.NavigateToType(typeof(Students), null, navOptions);
            //             //    break;
            //             //case "Teachers":
            //             //    ContentFrame.NavigateToType(typeof(Teachers), null, navOptions);
            //             //    break;
            //             //case "Settings":
            //             //    ContentFrame.NavigateToType(typeof(Settings), null, navOptions);
            //             //    break;
            //     }
                NavigateByTag(tag);
            }
        }

        private void NavigateByTag(string tag)
        {
            if (this.menu.MenuItems
                .OfType<NavigationViewItem>()
                .Where(x => x.Tag.Equals(tag) is true)
                .FirstOrDefault() is NavigationViewItem item)
            {
                this.menu.SelectedItem = item;
                var navigatingType = Type.GetType($"{item.Tag}");
                if (navigatingType is not null)
                {
                    this.ContentFrame.Navigate(navigatingType);
                }
            }
        }
    }
}
