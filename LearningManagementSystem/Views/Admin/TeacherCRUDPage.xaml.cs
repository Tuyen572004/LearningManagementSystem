#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using LearningManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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

namespace LearningManagementSystem.Views.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TeacherCRUDPage : Page, IDisposable
    {
        private readonly TeacherReaderViewModel _readerViewModel;
        private readonly TeacherCUDViewModel _cudViewModel;
        private bool disposedValue;

        private TableView InnerReaderTable => ReaderDisplayer.TableView;
        private TableView InnerCUDTable => CUDDisplayer.TableView;

        public TeacherCRUDPage()
        {
            this.InitializeComponent();

            _readerViewModel = new TeacherReaderViewModel(App.Current.Services.GetService<IDao>()!)
            {
                RowsPerPage = 5
            };
            _readerViewModel.LoadInitialItems();

            _cudViewModel = new TeacherCUDViewModel(App.Current.Services.GetService<IDao>()!)
            {
                RowsPerPage = 5
            };
            _cudViewModel.LoadInitialItems();

            InnerReaderTable.ItemDoubleTapped += _cudViewModel.ItemTransferHandler;
            InnerCUDTable.IsEditable = true;

            OnSelectedRemoving += _cudViewModel.ItemsRemoveHandler;
            AllSelectedItemsTransferred += _cudViewModel.ItemsTransferHandler;
            ItemCreationInitiated += _cudViewModel.ItemCreationHandler;
            ItemsUpdateInitiated += _cudViewModel.ItemsUpdateHandler;
            ItemsDeletionInitiated += _cudViewModel.ItemsDeleteHandler;
            OnItemsErrorsAdded += _cudViewModel.ItemsErrorsAddedHandler;

            _cudViewModel.ItemsSelectionChanged += InnerCUDTable.ItemsReselectionHandler;

            _cudViewModel.OnItemsUpdated += OnItemsUpdatedHandler;
            _cudViewModel.OnInvalidItemsTranferred += InvalidItemsTransferredHandler;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    InnerReaderTable.ItemDoubleTapped -= _cudViewModel.ItemTransferHandler;

                    OnSelectedRemoving -= _cudViewModel.ItemsRemoveHandler;
                    AllSelectedItemsTransferred -= _cudViewModel.ItemsTransferHandler;
                    ItemCreationInitiated -= _cudViewModel.ItemCreationHandler;
                    ItemsUpdateInitiated -= _cudViewModel.ItemsUpdateHandler;
                    ItemsDeletionInitiated -= _cudViewModel.ItemsDeleteHandler;
                    OnItemsErrorsAdded -= _cudViewModel.ItemsErrorsAddedHandler;

                    _cudViewModel.ItemsSelectionChanged -= InnerCUDTable.ItemsReselectionHandler;

                    _cudViewModel.OnItemsUpdated -= OnItemsUpdatedHandler;
                    _cudViewModel.OnInvalidItemsTranferred -= InvalidItemsTransferredHandler;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TeacherCRUDPage()
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

        private IList<object> GetSelectedItemsInCUD()
        {
            return CUDDisplayer.TableView.GetSelectedItems();
        }

        private IList<object> GetSelectedItemsInReader()
        {
            return ReaderDisplayer.TableView.GetSelectedItems();
        }

        static private (string identifierName, string value)? ProvideIdentityFor(object item)
        {
            if (item is Teacher teacher)
            {
                return ("Id", teacher.Id.ToString());
            }
            return null;
        }
        //static private bool IdentityCheck(object checkingItem, object existingItem)
        //{
        //    if (checkingItem is Teacher checkingTeacher && existingItem is Teacher existingTeacher)
        //    {
        //        return checkingTeacher.Id == existingTeacher.Id;
        //    }
        //    return false;
        //}
        private EventHandler<IList<object>>? CUDItemsReselectionHandler => CUDDisplayer.TableView.ItemsReselectionHandler;
        static private string ItemNamePascalCase => "Teacher";
        static private string ItemNameCamelCase => "teacher";

        private void ShowReaderInfoBar(InfoBarMessage message)
        {
            ReaderDisplayer.TableView.ShowInfoBar(message);
        }

        private void ShowCUDInfoBar(InfoBarMessage message)
        {
            CUDDisplayer.TableView.ShowInfoBar(message);
        }
        private void RefreshReaderData()
        {
            ReaderDisplayer.TableView.RefreshData();
        }

        // --- The following event handler and methods should not depend on the specific type of the item ---

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool _isAssigningUser = false;
        private void AssignUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAssigningUser)
            {
                return;
            }
            _isAssigningUser = true;

            //var selectedItems = GetSelectedItemsInCUD();
            //if (selectedItems.Count != 0)
            //{
            //    var ContentDialog = new ContentDialog
            //    {
            //        XamlRoot = this.XamlRoot,
            //        Title = "Assign User",
            //        PrimaryButtonText = "Assign",
            //        CloseButtonText = "Cancel"
            //    };
            //    Frame DialogFrame = new();
            //    DialogFrame.Navigate(typeof(UserAssignmentForTeachersPage), selectedItems);
            //    ContentDialog.PrimaryButtonClick += (s, args) =>
            //    {
            //        if (DialogFrame.Content is UserAssignmentForTeachersPage userAssignmentPage)
            //        {
            //            userAssignmentPage.UserAssignmentViewModel?.OnConfirmAssignment();
            //        }
            //    };
            //    ContentDialog.Content = DialogFrame;
            //    await ContentDialog.ShowAsync();
            //    _isAssigningUser = false;
            //}

            var selectedItems = GetSelectedItemsInCUD();
            if (selectedItems.Count != 0)
            {
                Frame.Navigate(typeof(UserAssignmentForTeachersPage), selectedItems);
            }
            else
            {
                ShowCUDInfoBar(new()
                {
                    Title = "User Assignment Warning",
                    Message = "Please select the teacher(s) to be assigned!",
                    Severity = InfoBarMessageSeverity.Warning
                });
            }
            _isAssigningUser = false;
        }

        public event EventHandler? ItemCreationInitiated;
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ItemCreationInitiated?.Invoke(this, new EventArgs());
        }
        public event EventHandler<IList<object>>? ItemsUpdateInitiated;
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInCUD();
            if (selectedItems.Count != 0)
            {
                ItemsUpdateInitiated?.Invoke(this, selectedItems);
            }
        }
        public event EventHandler<IList<object>>? ItemsDeletionInitiated;
        public void HandleItemsUpdated(object? sender, (
            IList<object> updatedItems,
            IList<(object item, IEnumerable<String> errors)> invalidItemsInfo
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
            CUDItemsReselectionHandler?.Invoke(
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
            
            ShowCUDInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Info
            });
            RefreshReaderData();
        }

        public event EventHandler<IList<(object student, IEnumerable<string> errors)>>? OnItemsErrorsAdded;
        public EventHandler<(
            IList<object> updatedItems,
            IList<(object item, IEnumerable<string> errors)> invalidStudentsInfo
            )> OnItemsUpdatedHandler => HandleItemsUpdated;
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInCUD();
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
                """:
                $"""
                Ignoring {invalidItems.Count} {ItemNameCamelCase}(s).
                They have already existed in the transferred table.
                """
                ;
            ShowReaderInfoBar(new()
            {
                Title = displayTitle,
                Message = displayMessage,
                Severity = InfoBarMessageSeverity.Warning
            });
        }
        public EventHandler<IList<object>> InvalidItemsTransferredHandler => HandleInvalidItemsTransfered;
        private void EditAllButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GetSelectedItemsInReader();
            if (selectedItems.Count != 0)
            {
                AllSelectedItemsTransferred?.Invoke(this, selectedItems);
            }
        }

        public event EventHandler<IList<object>>? OnSelectedRemoving;
        private void RemoveAllButton_Click(object _, RoutedEventArgs __)
        {
            var selectedItems = GetSelectedItemsInCUD();
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
    }

    
}
