using LearningManagementSystem.DataAccess;
using LearningManagementSystem.EModels;
using LearningManagementSystem.Helpers;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.ViewModels
{
    public class EnrollmentClassesViewModel : PropertyChangedClass
    {
        private IDao _dao; // Private field to hold the dao instance
        public ObservableCollection<EnrollmentClassViewModel> EnrolledClassesViewModel { get; set; }

        public EnrollmentClassesViewModel()
        {
            EnrolledClassesViewModel = new FullObservableCollection<EnrollmentClassViewModel>();
            _dao = new ESqlDao(); // Instantiate the dao instance
        }

        public EnrollmentClassesViewModel(IDao dao) // Constructor accepting IDao
        {
            _dao = dao; // Assign the dao instance to the private field
            EnrolledClassesViewModel = new FullObservableCollection<EnrollmentClassViewModel>();
        }

        public void LoadEnrolledClasses()
        {
            var studentId = 1; // Example student ID
            var enrolledClasses = _dao.GetEnrolledClassesByStudentId(studentId);

            foreach (var enrolledClass in enrolledClasses)
            {
                // get teachers
                var teachers = new ObservableCollection<Teacher>(_dao.GetTeachersByClassId(enrolledClass.Id));

                EnrolledClassesViewModel.Add(new EnrollmentClassViewModel
                {
                    Class = enrolledClass,
                    Teachers = teachers
                });
            }
        }
    }
}
