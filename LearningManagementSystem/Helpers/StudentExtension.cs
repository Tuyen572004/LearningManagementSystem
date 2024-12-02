using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace LearningManagementSystem.Models
{
    public partial class StudentVer2 : ICloneable
    {
        public static readonly int MAX_STUDENT_CODE_LENGTH = 10;
        public static readonly int MAX_STUDENT_NAME_LENGTH = 100;
        public static readonly int MAX_EMAIL_LENGTH = 100;
        public static readonly int MAX_PHONE_NO_LENGTH = 30;
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj is StudentVer2 other)
            {
                return 
                    StudentCode == other.StudentCode &&
                    StudentName == other.StudentName &&
                    Email == other.Email &&
                    BirthDate == other.BirthDate &&
                    PhoneNo == other.PhoneNo &&
                    UserId == other.UserId &&
                    EnrollmentYear == other.EnrollmentYear &&
                    GraduationYear == other.GraduationYear;
            }
            return false;
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

        static public StudentVer2 Empty()
        {
            return new()
            {
                Id = -1,
                StudentCode = "",
                StudentName = "",
                Email = "",
                BirthDate = DateTime.Now,
                PhoneNo = "",
                UserId = null,
                EnrollmentYear = DateTime.Now.Year,
                GraduationYear = null
            };
        }

        static public (bool result, IEnumerable<String> errors) FieldsCheck(StudentVer2 student)
        {
            List<String> errors = [];
            if (student.StudentCode.Length == 0)
            {
                errors.Add("Student code must not be empty.");
            }
            if (student.StudentName.Length == 0)
            {
                errors.Add("Student name must not be empty.");
            }
            if (student.Email.Length == 0)
            {
                errors.Add("Email must not be empty.");
            }
            else
            {
                if (!EmailRegex().IsMatch(student.Email))
                {
                    errors.Add("Email is not in a valid format.");
                }
            }

            if (student.PhoneNo.Length == 0)
            {
                errors.Add("Phone number must not be empty.");
            }
            else
            {
                if (!PhoneRegex().IsMatch(student.PhoneNo))
                {
                    errors.Add("Phone number is not in a valid format.");
                }
            }

            if (student.StudentCode.Length > StudentVer2.MAX_STUDENT_CODE_LENGTH)
            {
                errors.Add($"Student code must be less than {StudentVer2.MAX_STUDENT_CODE_LENGTH} characters.");
            }
            if (student.StudentName.Length > StudentVer2.MAX_STUDENT_NAME_LENGTH)
            {
                errors.Add($"Student name must be less than {StudentVer2.MAX_STUDENT_NAME_LENGTH} characters.");
            }
            if (student.Email.Length > StudentVer2.MAX_EMAIL_LENGTH)
            {
                errors.Add($"Email must be less than {StudentVer2.MAX_EMAIL_LENGTH} characters.");
            }
            if (student.PhoneNo.Length > StudentVer2.MAX_PHONE_NO_LENGTH)
            {
                errors.Add($"Phone number must be less than {StudentVer2.MAX_PHONE_NO_LENGTH} characters.");
            }

            if (student.EnrollmentYear < 1900 || student.EnrollmentYear > DateTime.Now.Year)
            {
                errors.Add("Enrollment year must be between 1900 and current year.");
            }
            if (student.GraduationYear != null && (student.GraduationYear < 1900 || student.GraduationYear > DateTime.Now.Year))
            {
                errors.Add("Graduation year must be between 1900 and current year.");
            }
            return (errors.Count == 0, errors);
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^\+?\d{0,3}[-.\s]?\(?\d+\)?[-.\s]?\d+([-.\s]?\d+)*$")]
        private static partial Regex PhoneRegex();
    }
}
