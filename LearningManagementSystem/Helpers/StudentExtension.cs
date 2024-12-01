using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public partial class StudentVer2 : ICloneable
    {
        public object Clone()
        {
            return new StudentVer2
            {
                Id = this.Id,
                StudentCode = this.StudentCode,
                StudentName = this.StudentName,
                Email = this.Email,
                BirthDate = this.BirthDate,
                PhoneNo = this.PhoneNo,
                UserId = this.UserId,
                EnrollmentYear = this.EnrollmentYear,
                GraduationYear = this.GraduationYear
            };
        }

        public StudentVer2 Copy(StudentVer2 other)
        {
            StudentCode = other.StudentCode;
            StudentName = other.StudentName;
            Email = other.Email;
            BirthDate = other.BirthDate;
            PhoneNo = other.PhoneNo;
            UserId = other.UserId;
            EnrollmentYear = other.EnrollmentYear;
            GraduationYear = other.GraduationYear;
            return this;
        }

        public StudentVer2 AsEditified()
        {
            StudentCode ??= "";
            StudentName ??= "";
            Email ??= "";
            PhoneNo ??= "";
            UserId ??= 0;
            GraduationYear ??= 0;
            return this;
        }
    }
}
