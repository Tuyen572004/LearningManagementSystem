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
        /// <summary>
        /// Retrieves all departments with pagination and optional keyword filtering.
        /// </summary>
        /// <param name="page">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <param name="keyword">The keyword to filter departments by code. Default is an empty string.</param>
        /// <param name="nameAscending">Specifies the order of department names. Default is false.</param>
        /// <returns>A tuple containing the total number of items and a list of departments.</returns>
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

        /// <summary>
        /// Inserts a new department into the database.
        /// </summary>
        /// <param name="department">The department to insert.</param>
        /// <returns>The ID of the newly inserted department.</returns>
        public int InsertDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="department">The department to update.</param>
        public void UpdateDepartment(Department department)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a department from the database by its ID.
        /// </summary>
        /// <param name="departmentId">The ID of the department to remove.</param>
        public void RemoveDepartmentByID(int departmentId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the ID of a department based on its code.
        /// </summary>
        /// <param name="department">The department to find the ID for.</param>
        /// <returns>The ID of the department.</returns>
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

        /// <summary>
        /// Counts the total number of departments in the database.
        /// </summary>
        /// <returns>The total number of departments.</returns>
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

        /// <summary>
        /// Retrieves a department by its ID.
        /// </summary>
        /// <param name="departmentId">The ID of the department to retrieve.</param>
        /// <returns>The department with the specified ID, or null if not found.</returns>
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
