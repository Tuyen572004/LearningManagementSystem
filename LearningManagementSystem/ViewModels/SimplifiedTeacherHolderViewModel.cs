using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
#nullable enable
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    partial class SimplifiedTeacherHolderViewModel(IDao dao): CUDViewModel(dao)
    {
        public override IEnumerable<string> ColumnOrder => ["TeacherCode", "TeacherName", "Email"];
        public override IEnumerable<string> IgnoringColumns => ["Id", "UserId", "IsValid", "PhoneNo"];

        public override InfoBarMessage? GetMessageOf(object item) => null;
        public override RowStatus? GetRowStatus(object item) => null;

        public void PopulateTeachers(IEnumerable<object> objects)
        {
            PopulateItems(objects.OfType<Teacher>());
        }
    }
}
