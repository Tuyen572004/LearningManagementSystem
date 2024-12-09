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
                    return "student";
                case Role.Teacher:
                    return "teacher";
                case Role.Admin:
                    return "admin";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
