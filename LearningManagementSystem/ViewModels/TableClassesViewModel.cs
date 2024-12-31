using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class TableClassesView : BaseViewModel, ICloneable
    {
        public int ID { get; set; }
        public string ClassCode { get; set; }
        public int CourseID { get; set; }
        public int CycleID { get; set; }
        public DateTime ClassStartDate { get; set; }
        public DateTime ClassEndDate { get; set; }

        public object Clone()
        {
            return new TableClassesView
            {
                ID = this.ID,
                CourseID = this.CourseID,
                CycleID = this.CycleID,
                ClassCode = this.ClassCode,
                ClassStartDate = this.ClassStartDate,
                ClassEndDate = this.ClassEndDate
            };
        }
    }
    public class TableClassesViewModel : BaseViewModel
    {
        private IDao _dao = null;

        public TableClassesView SelectedClass { get; set; }
        public FullObservableCollection<TableClassesView> TableClasses { get; set; }
        public List<string> Suggestion { get; set; }
        public string Keyword { get; set; } = "";
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "ASC";
        public int CurrentPage { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalItems { get; set; } = 0;

        public int countRepeatButton { get; set; } = 0;
        public TableClassesViewModel()
        {
            TableClasses = new FullObservableCollection<TableClassesView>();
            _dao = App.Current.Services.GetService<IDao>(); ;
            countRepeatButton = 0;
            SelectedClass = new TableClassesView();
            Suggestion = _dao.GetAllClassesCode();
        }
        public void GetAllClass()
        {
            var (totalItems, classes) = _dao.GetAllClasses(CurrentPage, RowsPerPage, Keyword, SortBy, SortOrder);
            if (classes != null)
            {
                TableClasses.Clear();
                for (int i = 0; i < classes.Count; i++)
                {
                    TableClasses.Add(new TableClassesView
                    {
                        CourseID = classes[i].CourseId,
                        ClassCode = classes[i].ClassCode,
                        ID = classes[i].Id,
                        ClassStartDate = classes[i].ClassStartDate,
                        ClassEndDate = classes[i].ClassEndDate
                    });
                }
                TotalItems = totalItems;
                TotalPages = (TotalItems / RowsPerPage)
                    + ((TotalItems % RowsPerPage == 0)
                            ? 0 : 1);
            }
        }

        public int InsertClass(Class newClass)
        {
            int count = _dao.InsertClass(newClass);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }


        public void Load(int page = 1)
        {
            CurrentPage = page;
            GetAllClass();
        }

        public void RemoveClass(Class newClass)
        {
            _dao.RemoveClassByID(newClass.Id);

        }
    }
}
