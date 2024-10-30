using LearningManagementSystem.DataAccess;
using LearningManagementSystem.ViewModels;
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

namespace LearningManagementSystem.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StudentDisplayerPage : Page
    {
        private readonly StudentsViewModel _viewModel = new(new MockDao());
        public StudentDisplayerPage()
        {
            this.InitializeComponent();
            QueryTest();
        }

        private void QueryTest()
        {
            _viewModel.MakeQuery()
                .ChangeRowsPerPage(7)
                .ChangePageNumber(2)
                .AddSortCriterias((StudentField.BirthDate, Ordering.Descending))
                .Execute();

            _viewModel.MakeQuery()
                .Reset()
                .AddSearchKeywords((StudentField.StudentName, "j"))
                .AddSortCriterias((StudentField.StudentCode, Ordering.Descending))
                .ModifySearchKeywords((StudentField.StudentName, "v"))
                .Execute();

            _viewModel.MakeQuery()
                .Reset()
                .AddFilterCriteria(StudentField.Id, 10, 21)
                .Execute();

            _viewModel.MakeQuery()
                .AddFilterCriteria(
                    StudentField.BirthDate,
                    new DateTime(2024, 05, 01),
                    new DateTime(2024, 07, 31),
                    true, true, true
                    )
                .AddSortCriterias((StudentField.BirthDate, Ordering.Ascending))
                .Execute();

            return;
        }
    }
}
