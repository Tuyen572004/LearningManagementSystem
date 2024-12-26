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
    public class ClassViewModel : BaseViewModel
    {
        private IDao _dao = null;

        public Class SelectedClass { get; set; }

        public ClassViewModel()
        {
            _dao = App.Current.Services.GetService<IDao>();
            SelectedClass = new Class();
        }
        public int InsertClass(Class newClass)
        {
            int count = _dao.InsertClass(newClass);
            return count;
        }

        public void UpdateClass(Class newClass)
        {
            _dao.UpdateClass(newClass);
        }

        public int CountClasses()
        {
            return _dao.CountClass();
        }
    };
}
