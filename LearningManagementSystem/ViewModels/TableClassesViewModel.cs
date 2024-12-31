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
    /// <summary>
    /// Represents a view model for table classes.
    /// </summary>
    public class TableClassesView : BaseViewModel, ICloneable
    {
        /// <summary>
        /// Gets or sets the ID of the class.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the class code.
        /// </summary>
        public string ClassCode { get; set; }

        /// <summary>
        /// Gets or sets the course ID.
        /// </summary>
        public int CourseID { get; set; }

        /// <summary>
        /// Gets or sets the cycle ID.
        /// </summary>
        public int CycleID { get; set; }

        /// <summary>
        /// Gets or sets the start date of the class.
        /// </summary>
        public DateTime ClassStartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the class.
        /// </summary>
        public DateTime ClassEndDate { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
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
    /// <summary>
    /// ViewModel for managing table classes.
    /// </summary>
    public class TableClassesViewModel : BaseViewModel
    {
        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the selected class.
        /// </summary>
        public TableClassesView SelectedClass { get; set; }

        /// <summary>
        /// Gets or sets the collection of table classes.
        /// </summary>
        public FullObservableCollection<TableClassesView> TableClasses { get; set; }

        /// <summary>
        /// Gets or sets the list of suggestions.
        /// </summary>
        public List<string> Suggestion { get; set; }

        /// <summary>
        /// Gets or sets the keyword for searching.
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// Gets or sets the column to sort by.
        /// </summary>
        public string SortBy { get; set; } = "Id";

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of rows per page.
        /// </summary>
        public int RowsPerPage { get; set; } = 10;

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Gets or sets the count of repeat button clicks.
        /// </summary>
        public int countRepeatButton { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableClassesViewModel"/> class.
        /// </summary>
        public TableClassesViewModel()
        {
            TableClasses = new FullObservableCollection<TableClassesView>();
            _dao = App.Current.Services.GetService<IDao>(); ;
            countRepeatButton = 0;
            SelectedClass = new TableClassesView();
            Suggestion = _dao.GetAllClassesCode();
        }

        /// <summary>
        /// Retrieves all classes based on the current filter and pagination settings.
        /// </summary>
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

        /// <summary>
        /// Inserts a new class into the database.
        /// </summary>
        /// <param name="newClass">The new class to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int InsertClass(Class newClass)
        {
            int count = _dao.InsertClass(newClass);

            if (count == 1)
            {
                //Courses.Add(course);
            }

            return count;
        }

        /// <summary>
        /// Loads the classes for the specified page.
        /// </summary>
        /// <param name="page">The page number to load.</param>
        public void Load(int page = 1)
        {
            CurrentPage = page;
            GetAllClass();
        }

        /// <summary>
        /// Removes a class from the database.
        /// </summary>
        /// <param name="newClass">The class to remove.</param>
        public void RemoveClass(Class newClass)
        {
            _dao.RemoveClassByID(newClass.Id);
        }
    }
}
