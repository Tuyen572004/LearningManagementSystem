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
    public sealed partial class StudentCRUDPage : Page
    {
        private readonly StudentReaderViewModel _readerViewModel;
        private readonly StudentCUDViewModel _cudViewModel;

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
        }
    }
}
