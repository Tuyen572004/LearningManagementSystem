using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using System;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.ViewModels
{
    public partial class StudentsInClassViewModel : BaseViewModel, IStudentProvider
    {
        // Enrollment
        public ObservableCollection<StudentVer2> ManagingStudents { get; private set; } = [];

        private Class _managingClass;
        private readonly IDao _dao;

        public Class ManagingClass
        {
            get => _managingClass;
            set
            {
                _managingClass = value;
                RaisePropertyChanged();
            }
        }

        public StudentsInClassViewModel(IDao dao)
        {
            _dao = dao;
            ManagingClass = new Class
            {
                Id = -1,
                ClassCode = "",
                CourseId = -1,
                CycleId = -1,
                ClassEndDate = new DateTime(),
                ClassStartDate = new DateTime(),
            };
        }

        public void LoadStudents()
        {
            ManagingStudents = _dao.GetStudentsByClassId(_managingClass.Id);
        }
    }
}
