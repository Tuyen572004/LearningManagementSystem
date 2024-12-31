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
    /// ViewModel for managing Class entities.
    /// </summary>
    public class ClassViewModel : BaseViewModel
    {
        private IDao _dao = null;

        /// <summary>
        /// Gets or sets the selected class.
        /// </summary>
        public Class SelectedClass { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassViewModel"/> class.
        /// </summary>
        public ClassViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();
            SelectedClass = new Class();
        }

        /// <summary>
        /// Inserts a new class into the database.
        /// </summary>
        /// <param name="newClass">The class to insert.</param>
        /// <returns>The number of rows affected.</returns>
        public int InsertClass(Class newClass)
        {
            int count = _dao.InsertClass(newClass);
            return count;
        }

        /// <summary>
        /// Updates an existing class in the database.
        /// </summary>
        /// <param name="newClass">The class to update.</param>
        public void UpdateClass(Class newClass)
        {
            _dao.UpdateClass(newClass);
        }

        /// <summary>
        /// Counts the total number of classes in the database.
        /// </summary>
        /// <returns>The total number of classes.</returns>
        public int CountClasses()
        {
            return _dao.CountClass();
        }
    }
}
