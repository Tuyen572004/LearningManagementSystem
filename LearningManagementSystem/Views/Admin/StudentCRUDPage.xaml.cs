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
using LearningManagementSystem.DataAccess;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LearningManagementSystem.Views.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StudentCRUDPage : Page, IDisposable
    {
        private readonly StudentReaderViewModel _readerViewModel;
        private readonly StudentCUDViewModel _cudViewModel;
        private bool disposedValue;

        public StudentCRUDPage()
        {
            this.InitializeComponent();

            _readerViewModel = new StudentReaderViewModel(new SqlDao())
            {
                RowsPerPage = 5
            };
            _readerViewModel.GetStudents();

            _cudViewModel = new StudentCUDViewModel(new SqlDao())
            {
                RowsPerPage = 5
            };

            StudentReaderDisplayer.StudentTable.StudentDoubleTapped += _cudViewModel.StudentTransferHandler;
            StudentCUDDisplayer.StudentTable.IsEditable = true;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StudentReaderDisplayer.StudentTable.StudentDoubleTapped -= _cudViewModel.StudentTransferHandler;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StudentCRUDPage()
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

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditAllButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
