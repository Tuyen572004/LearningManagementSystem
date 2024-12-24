using LearningManagementSystem.Services.UserService;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashBoard : Window
    {

        public UserService MyUserService { get; set; }
        //public static INavigationService NavigationService { get; private set; }
        public static IntPtr HWND { get; set; }
        public DashBoard()
        {
            this.InitializeComponent();
            InitializeAsync();
            HWND = WinRT.Interop.WindowNative.GetWindowHandle(this);
            NavigateByTag("LearningManagementSystem.HomePage");

            Menu.SelectionChanged += menu_SelectionChanged;
        }
        private async void InitializeAsync()
        {
            var userRole = await UserService.GetCurrentUserRole();
            SetMenuItemVisibilityFooter("LearningManagementSystem.Views.AdminPage", userRole == "admin");
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.StudentQueryPage", userRole == "admin" || userRole == "teacher");
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.StudentCRUDPage", userRole == "admin" || userRole == "teacher");
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.TeacherCRUDPage", userRole == "admin");
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.TeacherQueryPage", userRole == "admin");
        }
        private void SetMenuItemVisibilityFooter(string tag, bool isVisible)
        {
            var menuItem = Menu.FooterMenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag.ToString() == tag);
            if (menuItem != null)
            {
                menuItem.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetMenuItemVisibility(string tag, bool isVisible)
        {
            var menuItem = Menu.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag.ToString() == tag);
            if (menuItem != null)
            {
                menuItem.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void menu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item &&
                item.Tag is string tag)
            {
                NavigateByTag(tag);
            }
        }

        private void NavigateByTag(string tag)
        {
            var item = this.Menu.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(x => x.Tag.Equals(tag))
                ?? this.Menu.FooterMenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(x => x.Tag.Equals(tag));

            if (item != null)
            {
                this.Menu.SelectedItem = item;
                var navigatingType = Type.GetType($"{item.Tag}");
                if (navigatingType != null)
                {
                    this.ContentFrame.Navigate(navigatingType);
                }
            }
        }
    }
}
