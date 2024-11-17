using CommunityToolkit.Mvvm.Input;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;
using LearningManagementSystem.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mysqlx.Prepare;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.WebUI;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentClassViewModel : BaseViewModel
    {
        public Class Class { get; set; }

        public Course Course { get; set; }
        public ObservableCollection<Teacher> Teachers { get; set; }
        public Department Department { get; set; }

        public string ClassTitle => $"{Course.CourseCode}-{Course.CourseDescription} {Class.ClassCode}";
        public string DepartmentName => $"Dept. of {Department.DepartmentCode}-{Department.DepartmentDesc}";

        public EnrollmentClassViewModel()
        {
            Class = new Class();
            Course = new Course();
            Department = new Department();
            Teachers = new ObservableCollection<Teacher>();
        }

        public void loadClassByClassId(int classId)
        {
            var dao = new SqlDao();
            Class = dao.findClassById(classId);
            Course = dao.GetCourseById(Class.CourseId);
            Department = dao.GetDepartmentById(Course.DepartmentId);
            Teachers = new ObservableCollection<Teacher>(dao.GetTeachersByClassId(classId));
        }


    }
}
