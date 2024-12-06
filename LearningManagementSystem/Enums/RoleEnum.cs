using System;

namespace LearningManagementSystem.Enums
{
    public enum Role
    {
        Student,
        Teacher,
        Admin 
    }

    public static class RoleEnum
    {
        public static string GetStringValue(Role role)
        {
            switch (role)
            {
                case Role.Student:
                    return "Student";
                case Role.Teacher:
                    return "Teacher";
                case Role.Admin:
                    return "Admin";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
