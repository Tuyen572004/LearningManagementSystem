#nullable enable
using LearningManagementSystem.Controls;
using LearningManagementSystem.DataAccess;
using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.ViewModels
{
    partial class SimplifiedStudentHolderViewModel(IDao dao): CUDViewModel(dao)
    {
        public override IEnumerable<string> ColumnOrder => ["StudentCode", "StudentName", "Email"];
        public override IEnumerable<string> IgnoringColumns => ["Id", "UserId", "IsValid", "PhoneNo", "BirthDate", "GraduationYear", "EnrollmentYear"];
        public override InfoBarMessage? GetMessageOf(object item) => null;
        public override RowStatus? GetRowStatus(object item) => null;

        public void PopulateStudents(IEnumerable<object> objects)
        {
            PopulateItems(objects.OfType<StudentVer2>());
        }
    }
}
