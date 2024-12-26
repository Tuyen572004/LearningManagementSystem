#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using LearningManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Controls;
using LearningManagementSystem.Models;
using NPOI.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditStudentsInClassPage : Page, IDisposable
    {
        private SimpleClassViewModel? _simpleClassViewModel = null;
        private StudentsInClassViewModel? _studentsInClassViewModel = null;
        private StudentReaderViewModelVer2? _allStudentsViewModel = null;
        private StudentsInClassCUDViewModel? _studentsEnrollmentViewModel = null;
        private int _classId = -1;
        private bool disposedValue;

        private TableView InnerStudentsInClassTable => StudentsInClassTable.TableView;
        private TableView InnerAllStudentsTable => AllStudentsTable.TableView;
        private TableView InnerStudentsEnrollmentTable => StudentsEnrollmentTable.TableView;

        public EditStudentsInClassPage()
        {
            this.InitializeComponent();

            //var mockClassId = 14;

            //_studentsInClassViewModel = new StudentsInClassViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            //{
            //    RowsPerPage = 5
            //};
            //_studentsInClassViewModel.LoadInitialItems();
            //_allStudentsViewModel = new StudentReaderViewModelVer2(App.Current.Services.GetService<IDao>()!)
            //{
            //    RowsPerPage = 5
            //};
            //_allStudentsViewModel.LoadInitialItems();
            //_studentsEnrollmentViewModel = new StudentsInClassCUDViewModel(App.Current.Services.GetService<IDao>()!, mockClassId)
            //{
            //    RowsPerPage = 5
            //};
            //_studentsEnrollmentViewModel.LoadInitialItems();

            //InnerStudentsInClassTable.ItemDoubleTapped += _studentsEnrollmentViewModel.ItemTransferHandler;
            //InnerAllStudentsTable.ItemDoubleTapped += _studentsEnrollmentViewModel.ItemTransferHandler;

            //OnSelectedRemoving += _studentsEnrollmentViewModel.ItemsRemoveHandler;
            //AllSelectedItemsTransferred += _studentsEnrollmentViewModel.ItemsTransferHandler;
            //ItemsUpdateInitiated += _studentsEnrollmentViewModel.ItemsUpdateHandler;
            //ItemsDeletionInitiated += _studentsEnrollmentViewModel.ItemsDeleteHandler;
            //OnItemsErrorsAdded += _studentsEnrollmentViewModel.ItemsErrorsAddedHandler;

            //_studentsEnrollmentViewModel.ItemsSelectionChanged += InnerStudentsEnrollmentTable.ItemsReselectionHandler;

            //_studentsEnrollmentViewModel.OnItemsUpdated += OnItemsUpdatedHandler;
            //_studentsEnrollmentViewModel.OnInvalidItemsTranferred += InvalidItemsTransferredHandler;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // if (e.Parameter is int classId)
            if (14 is int classId)
            {
                _classId = classId;

                _simpleClassViewModel = new SimpleClassViewModel(App.Current.Services.GetService<IDao>()!, classId);
                _simpleClassViewModel.LoadRequiredInformation();

                _studentsInClassViewModel = new StudentsInClassViewModel(App.Current.Services.GetService<IDao>()!, classId)
                {
                    RowsPerPage = 5
                };
                _studentsInClassViewModel.LoadInitialItems();
                _allStudentsViewModel = new StudentReaderViewModelVer2(App.Current.Services.GetService<IDao>()!)
                {
                    RowsPerPage = 5
                };
                _allStudentsViewModel.LoadInitialItems();
                _studentsEnrollmentViewModel = new StudentsInClassCUDViewModel(App.Current.Services.GetService<IDao>()!, classId)
                {
                    RowsPerPage = 5
                };
                _studentsEnrollmentViewModel.LoadInitialItems();

                InnerStudentsInClassTable.ItemDoubleTapped += _studentsEnrollmentViewModel.ItemTransferHandler;
                InnerAllStudentsTable.ItemDoubleTapped += _studentsEnrollmentViewModel.ItemTransferHandler;

                OnSelectedRemoving += _studentsEnrollmentViewModel.ItemsRemoveHandler;
                AllSelectedItemsTransferred += _studentsEnrollmentViewModel.ItemsTransferHandler;
                ItemsUpdateInitiated += _studentsEnrollmentViewModel.ItemsUpdateHandler;
                ItemsDeletionInitiated += _studentsEnrollmentViewModel.ItemsDeleteHandler;
                OnItemsErrorsAdded += _studentsEnrollmentViewModel.ItemsErrorsAddedHandler;

                _studentsEnrollmentViewModel.ItemsSelectionChanged += InnerStudentsEnrollmentTable.ItemsReselectionHandler;

                _studentsEnrollmentViewModel.OnItemsUpdated += OnItemsUpdatedHandler;
                _studentsEnrollmentViewModel.OnInvalidItemsTranferred += InvalidItemsTransferredHandler;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _studentsEnrollmentViewModel is not null)
                {
                    InnerStudentsInClassTable.ItemDoubleTapped -= _studentsEnrollmentViewModel.ItemTransferHandler;
                    InnerAllStudentsTable.ItemDoubleTapped -= _studentsEnrollmentViewModel.ItemDoubleTappedHandler;

                    OnSelectedRemoving -= _studentsEnrollmentViewModel.ItemsRemoveHandler;
                    AllSelectedItemsTransferred -= _studentsEnrollmentViewModel.ItemsTransferHandler;
                    ItemsUpdateInitiated -= _studentsEnrollmentViewModel.ItemsUpdateHandler;
                    ItemsDeletionInitiated -= _studentsEnrollmentViewModel.ItemsDeleteHandler;
                    OnItemsErrorsAdded -= _studentsEnrollmentViewModel.ItemsErrorsAddedHandler;

                    _studentsEnrollmentViewModel.ItemsSelectionChanged -= InnerStudentsEnrollmentTable.ItemsReselectionHandler;

                    _studentsEnrollmentViewModel.OnItemsUpdated -= OnItemsUpdatedHandler;
                    _studentsEnrollmentViewModel.OnInvalidItemsTranferred -= InvalidItemsTransferredHandler;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EditStudentsInClassPage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private IList<object> GetSelectedItemsInClass()
        {
            return InnerStudentsInClassTable.GetSelectedItems();
        }
        private IList<object> GetSelectedItemsInAll()
        {
            return InnerAllStudentsTable.GetSelectedItems();
        }

        private IList<object> GetSelectedItemsInCD()
        {
            return InnerStudentsEnrollmentTable.GetSelectedItems();
        }

        static private (string identifierName, string value)? ProvideIdentityFor(object item)
        {
            if (item is StudentVer2 student)
            {
                return ("Id", student.Id.ToString());
            }
            return null;
        }
        private EventHandler<IList<object>>? CDItemsReselectionHandler => InnerStudentsEnrollmentTable.ItemsReselectionHandler;
        static private string ItemNamePascalCase => "Student";
        static private string ItemNameCamelCase => "student";

        private void ShowInClassInfoBar(InfoBarMessage message)
        {
            StudentsInClassTable.TableView.ShowInfoBar(message);
        }
        //private void ShowAllInfoBar(InfoBarMessage message)
        //{
        //    AllStudentsTable.TableView.ShowInfoBar(message);
        //}
        private void ShowCDInfoBar(InfoBarMessage message)
        {
            StudentsEnrollmentTable.TableView.ShowInfoBar(message);
        }

        private void RefreshInClassData()
        {
            StudentsInClassTable.TableView.RefreshData();
        }

        // --- The following event handler and methods should not depend on the specific type of the item ---

        public event EventHandler<IList<object>>? ItemsUpdateInitiated;
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInCD();
            if (selectedItems.Count != 0)
            {
                ItemsUpdateInitiated?.Invoke(this, selectedItems);
            }
        }
        public event EventHandler<IList<object>>? ItemsDeletionInitiated;
        public void HandleItemsUpdated(object? sender, (
            IList<object> updatedItems,
            IList<(object item, IEnumerable<string> errors)> invalidItemsInfo
            ) e)
        {
            if (sender is null)
            {
                return;
            }
            if (e.updatedItems.Count > 0)
            {
                OnSelectedRemoving?.Invoke(this, e.updatedItems);
            }
            if (e.invalidItemsInfo.Count > 0)
            {
                OnItemsErrorsAdded?.Invoke(this, e.invalidItemsInfo);
            }
            CDItemsReselectionHandler?.Invoke(
                this,
                e.invalidItemsInfo.Select(info => info.item).ToList()
                );

            var displayTitle = $"{ItemNamePascalCase}(s) Updated";
            if (e.updatedItems.Count == 0)
            {
                displayTitle += "?!";
            }
            if (e.invalidItemsInfo.Count > 0)
            {
                displayTitle += " with failure(s)";
            }

            var displayMessage = e.invalidItemsInfo.Count > 0
                ? $"{e.updatedItems.Count} {ItemNameCamelCase}(s) have been modified successfully. \n{e.invalidItemsInfo.Count} {ItemNameCamelCase}(s) have errors."
                : $"{e.updatedItems.Count} {ItemNameCamelCase}(s) have been modified successfully.";

            ShowCDInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Info
            });
            RefreshInClassData();
        }

        public event EventHandler<IList<(object student, IEnumerable<string> errors)>>? OnItemsErrorsAdded;
        public EventHandler<(
            IList<object> updatedItems,
            IList<(object item, IEnumerable<string> errors)> invalidStudentsInfo
            )> OnItemsUpdatedHandler => HandleItemsUpdated;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInCD();
            if (selectedItems.Count != 0)
            {
                ItemsDeletionInitiated?.Invoke(this, selectedItems);
            }
        }

        public event EventHandler<IList<object>>? AllSelectedItemsTransferred;

        public void HandleInvalidItemsTransfered(object? sender, IList<object> invalidItems)
        {
            if (sender is null)
            {
                return;
            }
            // var invalidIds = invalidStudents.Select(student => student.Id).ToList();

            List<string> invalidIdentifiers = [];
            string? currentIdentifierName = null;
            foreach (var item in invalidItems)
            {
                var identityInfo = ProvideIdentityFor(item);
                if (identityInfo is null)
                {
                    invalidIdentifiers.Add("???");
                    continue;
                }
                currentIdentifierName ??= identityInfo.Value.identifierName;
                if (currentIdentifierName != identityInfo.Value.identifierName)
                {
                    invalidIdentifiers.Add("???");
                    continue;
                }
                invalidIdentifiers.Add(identityInfo.Value.value);
            }

            var displayTitle = $"{ItemNamePascalCase} Transfering Warning";

            var displayMessage = currentIdentifierName is not null ?
                $"""
                Ignoring {invalidItems.Count} {ItemNameCamelCase}(s) with the following {currentIdentifierName}(s): {String.Join(", ", invalidIdentifiers)}.
                They have already existed in the transferred table.
                """ :
                $"""
                Ignoring {invalidItems.Count} {ItemNameCamelCase}(s).
                They have already existed in the transferred table.
                """
                ;
            ShowInClassInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Warning
            });
        }
        public EventHandler<IList<object>> InvalidItemsTransferredHandler => HandleInvalidItemsTransfered;
        private void EditAllButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInClass().ToList();
            selectedItems.AddRange(GetSelectedItemsInAll().AsEnumerable());
            if (selectedItems.Count != 0)
            {
                AllSelectedItemsTransferred?.Invoke(this, selectedItems);
            }
        }

        public event EventHandler<IList<object>>? OnSelectedRemoving;
        private void RemoveAllButton_Click(object _, RoutedEventArgs __)
        {
            var selectedItems = GetSelectedItemsInCD();
            if (selectedItems.Count != 0)
            {
                OnSelectedRemoving?.Invoke(this, selectedItems);
            }
        }
        private void HandleItemsChanged(object? sender, IList<object> selectedItems)
        {
            if (sender is null)
            {
                return;
            }
            if (selectedItems.Count != 0)
            {
                OnSelectedRemoving?.Invoke(this, selectedItems);
            }
        }
        public EventHandler<IList<object>> ItemsChangedHandler => HandleItemsChanged;

        private void AllStudentsTableToggler_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                AllStudentsTable.Visibility = Visibility.Visible;
            }
        }

        private void AllStudentsTableToggler_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                AllStudentsTable.Visibility = Visibility.Collapsed;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
