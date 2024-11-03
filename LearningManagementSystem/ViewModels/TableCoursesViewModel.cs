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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using LearningManagementSystem.Helper;
using LearningManagementSystem.DataAccess;

namespace LearningManagementSystem.ViewModels
{
    public class TableCoursesView : BaseViewModel, ICloneable
    {
        public int ID { get; set; }
        public string CourseCode { get; set; }
        public string CourseDecription { get; set; }
        public int TotalStudents { get; set; }
        public int TotalClasses { get; set; }

        public int DepartmentID { get; set; }
        public object Clone()
        {
            return new TableCoursesView
            {
                ID = this.ID,
                CourseCode = this.CourseCode,
                CourseDecription = this.CourseDecription,
                DepartmentID=this.DepartmentID,
                TotalStudents = this.TotalStudents,
                TotalClasses = this.TotalClasses
            };
        }
    }
    public class TableCoursesViewModel : BaseViewModel
    {
        private IDao _dao = null;
       
        public TableCoursesView SelectedCourse { get; set; }
        public FullObservableCollection<TableCoursesView> TableCourses { get; set; }

       
        public TableCoursesViewModel()
        {
            TableCourses = new FullObservableCollection<TableCoursesView>();
            _dao = new SqlDao();
            SelectedCourse = new TableCoursesView();

        }
        public void GetAllCourse()
        {
            var (totalItems, courses) = _dao.GetAllCourses();
            if (courses != null)
            {
                TableCourses.Clear();
                for (int i = 0; i < courses.Count; i++)
                {
                    TableCourses.Add(new TableCoursesView
                    {
                        ID = courses[i].Id,
                        CourseCode = courses[i].CourseCode,
                        CourseDecription = courses[i].CourseDescription,
                        DepartmentID= courses[i].DepartmentId,
                        TotalStudents = 0,
                        TotalClasses = 0
                    });
                }
            }
        }

        public int InsertCourse(Course course)
        {
            int count = _dao.InsertCourse(course);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }

        public void Load(int page)
        {
     
            GetAllCourse();
        }

        public void RemoveCourse(Course course)
        {
            _dao.RemoveCourseByID(course.Id);

        }
    }
}