using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Enums;
using LearningManagementSystem.Messages;
using LearningManagementSystem.Models;
using LearningManagementSystem.Services;
using LearningManagementSystem.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace LearningManagementSystem.Views
{
    public sealed partial class ClassDetailPage : Page
    {
        public ClassDetailViewModel ClassDetailViewModel { get; set; }

        public ClassDetailPage()
        {
            this.InitializeComponent();

            ClassDetailViewModel = new ClassDetailViewModel();

            this.DataContext = this;

            // r : receiver, m : message
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, (r, m) =>
            {
                Frame.Navigate(m.DestinationPage, m.Parameter);
            });

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
            //var classViewModel = e.Parameter as EnrollmentClassViewModel;
            //DataContext = this;
            base.OnNavigatedTo(e);

            if (e.Parameter is EnrollmentClassViewModel enrollmentViewModel) // navigated by EnrollmentClassesPage (click Enter button)
            {
                ClassDetailViewModel.EnrollmentViewModel = enrollmentViewModel;
                // Eager loading
                ClassDetailViewModel.ResourceViewModel.LoadMoreItems(ClassDetailViewModel.EnrollmentViewModel.Class.Id);

            }
            else if (e.Parameter is int classId) // navigated by Resources Page (click Back button)
            {
                ClassDetailViewModel.EnrollmentViewModel.loadClassByClassId(classId);
                // Eager loading
                ClassDetailViewModel.ResourceViewModel.LoadMoreItems(ClassDetailViewModel.EnrollmentViewModel.Class.Id);
            }
            
            

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Forward)
            //{
                WeakReferenceMessenger.Default.Unregister<NavigationMessage>(this);
                
            //}
        }


        public void Title_DoubleTapped(object sender,DoubleTappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var parameter = textBlock.DataContext as BaseResourceViewModel;
            var resource = parameter.BaseResource;
            if (resource != null)
            {
                if (resource is Assignment)
                {
                    var assignment = resource as Assignment;
                    Frame.Navigate(typeof(AssignmentPage), assignment);
                }
                // ------------ IMPLEMENT LATER ------------
                else if (resource is Notification)
                {
                    var notification = resource as Notification;
                    Frame.Navigate(typeof(NotificationPage), notification);
                }
                else if (resource is Document)
                {
                    var document = resource as Document;
                    Frame.Navigate(typeof(DocumentPage), document);
                }
            }
        }

        // IMPLEMENT LATER
        public void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EnrollmentClassesPage));
        }
    }
}

