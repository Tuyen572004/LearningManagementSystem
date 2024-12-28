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

            var result = new ObservableCollection<Class>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    SELECT c.id, c.courseid, c.classcode, c.cycleid, c.classstartdate, c.classenddate
                    FROM classes c
                    JOIN enrollments e ON c.id = e.classid
                    WHERE e.studentid = @StudentId"
                ;
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentId", studentId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Class
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CourseId = reader.GetInt32(reader.GetOrdinal("courseid")),
                                ClassCode = reader.GetString(reader.GetOrdinal("classcode")),
                                CycleId = reader.GetInt32(reader.GetOrdinal("cycleid")),
                                ClassStartDate = reader.GetDateTime(reader.GetOrdinal("classstartdate")),
                                ClassEndDate = reader.GetDateTime(reader.GetOrdinal("classenddate"))
                            });
                        }
                    }
                }
            }
            return result;

        }

        public ObservableCollection<Class> GetEnrolledClassesByTeacherId(int teacherId)
        {
            var result = new ObservableCollection<Class>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    SELECT c.id, c.courseid, c.classcode, c.cycleid, c.classstartdate, c.classenddate
                    FROM classes c
                    JOIN teachersperclass e ON c.id = e.classid
                    WHERE e.teacherid = @TeacherId"
                ;
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeacherId", teacherId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Class
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                CourseId = reader.GetInt32(reader.GetOrdinal("courseid")),
                                ClassCode = reader.GetString(reader.GetOrdinal("classcode")),
                                CycleId = reader.GetInt32(reader.GetOrdinal("cycleid")),
                                ClassStartDate = reader.GetDateTime(reader.GetOrdinal("classstartdate")),
                                ClassEndDate = reader.GetDateTime(reader.GetOrdinal("classenddate"))
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}
