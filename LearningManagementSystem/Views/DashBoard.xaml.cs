using LearningManagementSystem.Enums;
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
        /// <summary>
        /// Gets or sets the user service.
        /// </summary>
        public UserService MyUserService { get; set; }

        //public static INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Gets or sets the window handle.
        /// </summary>
        public static IntPtr HWND { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashBoard"/> class.
        /// </summary>
        public DashBoard()
        {
            Title = "Learning Management System";
            this.InitializeComponent();
            InitializeAsync();
            HWND = WinRT.Interop.WindowNative.GetWindowHandle(this);
            NavigateByTag("LearningManagementSystem.HomePage");
            MyUserService = new UserService();
            Menu.SelectionChanged += menu_SelectionChanged;
        }

        /// <summary>
        /// Initializes the dashboard asynchronously.
        /// </summary>
        private async void InitializeAsync()
        {
            var userRole = await UserService.GetCurrentUserRole();
            SetMenuItemVisibilityFooter("LearningManagementSystem.Views.AdminPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.StudentQueryPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.CoursesPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.StudentCRUDPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.TeacherCRUDPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.Admin.TeacherQueryPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.ClassesPage", checkAdmin(userRole));
            SetMenuItemVisibility("LearningManagementSystem.Views.EnrollmentClassesPage", !checkAdmin(userRole));
        }

        /// <summary>
        /// Checks if the user role is admin.
        /// </summary>
        /// <param name="userRole">The user role.</param>
        /// <returns><c>true</c> if the user role is admin; otherwise, <c>false</c>.</returns>
        private bool checkAdmin(string userRole)
        {
            return userRole.Equals(RoleEnum.GetStringValue(Role.Admin));
        }

        /// <summary>
        /// Sets the visibility of a footer menu item.
        /// </summary>
        /// <param name="tag">The tag of the menu item.</param>
        /// <param name="isVisible">if set to <c>true</c> the menu item is visible; otherwise, it is collapsed.</param>
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

        /// <summary>
        /// Sets the visibility of a menu item.
        /// </summary>
        /// <param name="tag">The tag of the menu item.</param>
        /// <param name="isVisible">if set to <c>true</c> the menu item is visible; otherwise, it is collapsed.</param>
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

        /// <summary>
        /// Handles the selection changed event of the menu.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NavigationViewSelectionChangedEventArgs"/> instance containing the event data.</param>
        private async void menu_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item &&
                item.Tag is string tag)
            {
                if (tag == "LearningManagementSystem.Logout")
                {
                    var dialog = new ContentDialog
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = "Log out",
                        Content = "Are you sure you want to log out?",
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No"
                    };
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        MyUserService.Logout();
                        this.Close();
                    }
                    else
                    {
                        dialog.Hide();
                    }

                }
                else if (tag == "LearningManagementSystem.Exit")
                {
                    var dialog = new ContentDialog
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Title = "Exit",
                        Content = "Are you sure you want to exit?",
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No"
                    };

                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        dialog.Hide();
                    }
                }
                else
                    NavigateByTag(tag);
            }
        }

        /// <summary>
        /// Navigates to the page specified by the tag.
        /// </summary>
        /// <param name="tag">The tag of the page to navigate to.</param>
        private void NavigateByTag(string tag)
        {
            if (tag == null)
                return;
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
