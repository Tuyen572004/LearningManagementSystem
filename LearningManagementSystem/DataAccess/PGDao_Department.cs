using CloudinaryDotNet;
using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        public Tuple<int, List<Department>> GetAllDepartments(int page = 1, int pageSize = 10, string keyword = "", bool nameAscending = false)
        {
            var result = new List<Department>();
            var sql = """
                SELECT COUNT(*) OVER() AS TotalItems, Id, DepartmentCode, DepartmentDesc
                FROM Departments
                WHERE DepartmentCode LIKE @Keyword
                ORDER BY Id ASC
                LIMIT @Take OFFSET @Skip
                """;

            var skip = (page - 1) * pageSize;
            var take = pageSize;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Skip", skip);
                command.Parameters.AddWithValue("@Take", take);
                command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                connection.Open();
                var reader = command.ExecuteReader();

                int totalItems = -1;
                while (reader.Read())
                {
                    if (totalItems == -1)
                    {
                        totalItems = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                    }
                    result.Add(new Department
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")),
                        DepartmentDesc = reader.GetString(reader.GetOrdinal("DepartmentDesc"))
                    });
                }
                return new Tuple<int, List<Department>>(totalItems, result);
            }
        }

        public int InsertDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public void UpdateDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        public void RemoveDepartmentByID(int departmentId)
        {
            throw new NotImplementedException();
        }

        public int FindDepartmentID(Department department)
        {
            var sql = "SELECT Id FROM Departments WHERE DepartmentCode=@DepartmentCode";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@DepartmentCode", department.DepartmentCode);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    department.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                }
                return department.Id;
            }
        }

        public int CountDepartments()
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Departments";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(reader.GetOrdinal("TotalItems"));
                return result;
            }
        }

        public Department GetDepartmentById(int departmentId)
        {
            var sql = "SELECT Id, DepartmentCode, DepartmentDesc FROM Departments WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", departmentId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return new Department
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")),
                        DepartmentDesc = reader.GetString(reader.GetOrdinal("DepartmentDesc"))
                    };
                }
                return null;
            }
        }
    }
}
