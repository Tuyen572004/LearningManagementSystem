using LearningManagementSystem.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace LearningManagementSystem.Helper
{
    public class DepartmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Department department)
            {
                return $"{department.DepartmentCode} - {department.DepartmentDesc}";
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}