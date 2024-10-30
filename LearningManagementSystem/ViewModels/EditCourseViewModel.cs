using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    public class EditCourseViewModel : CourseViewModel, ICloneable
    {
       public CourseViewModel _oldData { get; set; }
       public CourseViewModel _selectedData { get; set; }
        public EditCourseViewModel()
        {
            _oldData = new CourseViewModel();
            _selectedData = new CourseViewModel();
        }

        public object Clone()
        {
            return new EditCourseViewModel
            {
                _oldData = this._oldData,
                _selectedData = this._selectedData
            };
        }
    }
}
