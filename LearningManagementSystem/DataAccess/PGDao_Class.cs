using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        public Class findClassById(int classId)
        {
            var result = new Class();
            var sql = """
                SELECT Id, CourseId, ClassCode, CycleId, ClassStartDate, ClassEndDate
                FROM Classes
                WHERE Id=@classId
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassId", classId);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new Class
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        ClassCode = reader.GetString(reader.GetOrdinal("ClassCode")),
                        CycleId = reader.GetInt32(reader.GetOrdinal("CycleId")),
                        ClassStartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                        ClassEndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate"))
                    };
                }
            }
            return result;
        }

        public List<String> GetAllClassesCode()
        {
            var result = new List<String>();
            var sql = "SELECT ClassCode FROM Classes";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(reader.GetOrdinal("ClassCode")));
                }
            }
            return result;
        }
        public Tuple<int, List<Class>> GetAllClasses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<Class>();
            var sql = $"""
                SELECT COUNT(*) OVER() AS TotalItems, Id, ClassCode, CourseId, CycleID, ClassStartDate, ClassEndDate
                FROM Classes
                WHERE ClassCode LIKE @Keyword
                ORDER BY {sortBy} {sortOrder}
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
                    result.Add(new Class
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        ClassCode = reader.GetString(reader.GetOrdinal("ClassCode")),
                        CycleId = reader.GetInt32(reader.GetOrdinal("CycleId")),
                        ClassStartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                        ClassEndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate"))
                    });
                }
                return new Tuple<int, List<Class>>(totalItems, result);
            }
        }
        public void RemoveClassByID(int id)
        {
            var sql = "DELETE FROM Classes WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateClass(Class newClass)
        {
            var sql = "UPDATE Classes SET ClassCode=@ClassCode, CourseID=@CourseID, CycleID=@CycleID, ClassStartDate=@ClassStartDate, ClassEndDate=@ClassEndDate WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassCode", newClass.ClassCode);
                command.Parameters.AddWithValue("@CourseID", newClass.CourseId);
                command.Parameters.AddWithValue("@CycleID", newClass.CycleId);
                command.Parameters.AddWithValue("@ClassStartDate", newClass.ClassStartDate.Date);
                command.Parameters.AddWithValue("@ClassEndDate", newClass.ClassEndDate.Date);
                command.Parameters.AddWithValue("@Id", newClass.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public int CountClass()
        {
            var sql = "SELECT COUNT(*) AS TotalItems FROM Classes";
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

        public int InsertClass(Class newClass)
        {
            var sql = """
                INSERT INTO classes (classcode, courseid, cycleid, classstartdate, classenddate) 
                VALUES (@ClassCode, @CourseID, @CycleID, @ClassStartDate, @ClassEndDate)
                 returning id;
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassCode", newClass.ClassCode);
                command.Parameters.AddWithValue("@CourseID", newClass.CourseId);
                command.Parameters.AddWithValue("@CycleID", newClass.CycleId);
                command.Parameters.AddWithValue("@ClassStartDate", newClass.ClassStartDate.Date);
                command.Parameters.AddWithValue("@ClassEndDate", newClass.ClassEndDate.Date);

                connection.Open();
                int id = (int)command.ExecuteScalar();
                newClass.Id = id;

                return id > 0 ? 1 : -1;
            }
        }
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            return new ObservableCollection<Class>
            {
                new Class
                {
                    Id = 1,
                    CourseId = 1,
                    ClassCode = "CQ_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 2,
                    CourseId = 2,
                    ClassCode = "CQ_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 3,
                    CourseId = 3,
                    ClassCode = "HP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 4,
                    CourseId = 4,
                    ClassCode = "HP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 5,
                    CourseId = 5,
                    ClassCode = "HP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 6,
                    CourseId = 6,
                    ClassCode = "HP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 7,
                    CourseId = 7,
                    ClassCode = "HP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 8,
                    CourseId = 8,
                    ClassCode = "GP_1",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 9,
                    CourseId = 9,
                    ClassCode = "GP_2",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 10,
                    CourseId = 10,
                    ClassCode = "GP_3",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 11,
                    CourseId = 11,
                    ClassCode = "GP_4",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 12,
                    CourseId = 12,
                    ClassCode = "GP_5",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 13,
                    CourseId = 13,
                    ClassCode = "GP_6",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 14,
                    CourseId = 14,
                    ClassCode = "GP_7",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                },
                new Class
                {
                    Id = 15,
                    CourseId = 15,
                    ClassCode = "GP_8",
                    CycleId = 1,
                    ClassStartDate = new DateTime(2022, 1, 1),
                    ClassEndDate = new DateTime(2022, 5, 1)
                }
            };
        }


    }
}
