using LearningManagementSystem.DataAccess;
using LearningManagementSystem.EModels;
using LearningManagementSystem.Helpers;
using System.Collections.ObjectModel;
using System.Linq;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentClassViewModel : PropertyChangedClass
    {
        public Class Class { get; set; }

        public ObservableCollection<Teacher> Teachers { get; set; }

        public string ClassTitle => $"{Class.Course.CourseCode}-{Class.Course.CourseDescription} {Class.ClassCode}";
        public string DepartmentName => $"Dept. of {Class.Course.Department.DepartmentCode}-{Class.Course.Department.DepartmentDesc}";

        private readonly IDao _dao;

        public EnrollmentClassViewModel()
        {
            Class = new Class();
            Teachers = new ObservableCollection<Teacher>();
            _dao = new ESqlDao();
        }

        public void LoadClassByClassId(int classId)
        {
            Class = _dao.findClassById(classId);
            Teachers = new ObservableCollection<Teacher>(_dao.GetTeachersByClassId(classId));
        }
    }
}
