using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Models;

namespace LearningManagementSystem.ViewModels
{
    public class EnrolledClassViewModel : BaseViewModel
    {
        public Class Class { get; set; }
        public ObservableCollection<Teacher> teachers { get; set; }
        public Department Department { get; set; }
    }
    public class EnrollmentClassesViewModel : BaseViewModel
    {
       public ObservableCollection<EnrolledClassViewModel> enrolledClasses { get; set; }
        //public EnrollmentClassesViewModel()
        //{
        //    enrolledClasses = new ObservableCollection<EnrolledClassViewModel>();
        //    LoadEnrolledClasses();
        //}
        //private void LoadEnrolledClasses()
        //{
        //    var studentId = 1; // Example student ID
        //    var enrolledClasses = dao.GetEnrolledClassesByStudentId(studentId);

        //    foreach (var enrolledClass in enrolledClasses)
        //    {



                  
                

        //        enrolledClasses.Add(new EnrolledClassViewModel
        //        {
        //        });
        //    }
        //}
    }

    
}
