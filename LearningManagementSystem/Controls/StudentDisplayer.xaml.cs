using LearningManagementSystem.Models;
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
using System.Collections.ObjectModel;
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
    public interface IStudentProvider: INotifyPropertyChanged
    {
        public ObservableCollection<StudentVer2> ManagingStudents { get; }
    }
    public partial class UnassignedStudentProvider : IStudentProvider
    {
        public ObservableCollection<StudentVer2> ManagingStudents { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public sealed partial class StudentDisplayer : UserControl
    {
        // private readonly IStudentProvider _dataContext = null;
        public static readonly DependencyProperty ContextProviderProperty = 
            DependencyProperty.Register(
                "ContextProvider",
                typeof(IStudentProvider),
                typeof(StudentDisplayer),
                new PropertyMetadata(new UnassignedStudentProvider(), OnDataChanged)
                );

        public IStudentProvider ContextProvider
        {
            get => (IStudentProvider)GetValue(ContextProviderProperty);
            set => SetValue(ContextProviderProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Do nothing currently
        }

        public StudentDisplayer()
        {
            this.InitializeComponent();

            // _dataContext = ContextProvider;

            //if (_dataContext is null)
            //{
            //    throw new ArgumentNullException("DataContext cannot be null");
            //}
        }

    }
}
