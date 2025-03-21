﻿using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        /// <summary>
        /// Finds a class by its ID.
        /// </summary>
        /// <param name="classId">The ID of the class to find.</param>
        /// <returns>The class with the specified ID.</returns>
        public Class findClassById(int classId)
        {
            var result = new Class();
            var sql = """
                    SELECT Id, CourseId, ClassCode, ClassStartDate, ClassEndDate
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
                        ClassStartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                        ClassEndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate"))
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all class codes.
        /// </summary>
        /// <returns>A list of all class codes.</returns>
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

        /// <summary>
        /// Gets all classes with pagination and filtering options.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="keyword">The keyword to filter by class code.</param>
        /// <param name="sortBy">The column to sort by.</param>
        /// <param name="sortOrder">The sort order (ASC or DESC).</param>
        /// <returns>A tuple containing the total number of items and a list of classes.</returns>
        public Tuple<int, List<Class>> GetAllClasses(int page = 1, int pageSize = 10, string keyword = "", string sortBy = "Id", string sortOrder = "ASC")
        {
            var result = new List<Class>();
            var sql = $"""
                    SELECT COUNT(*) OVER() AS TotalItems, Id, ClassCode, CourseId, ClassStartDate, ClassEndDate
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
                        ClassStartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                        ClassEndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate"))
                    });
                }
                return new Tuple<int, List<Class>>(totalItems, result);
            }
        }

        /// <summary>
        /// Removes a class by its ID.
        /// </summary>
        /// <param name="id">The ID of the class to remove.</param>
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

        /// <summary>
        /// Updates an existing class.
        /// </summary>
        /// <param name="newClass">The class object with updated values.</param>
        public void UpdateClass(Class newClass)
        {
            var sql = "UPDATE Classes SET ClassCode=@ClassCode, CourseID=@CourseID, ClassStartDate=@ClassStartDate, ClassEndDate=@ClassEndDate WHERE Id=@Id";
            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassCode", newClass.ClassCode);
                command.Parameters.AddWithValue("@CourseID", newClass.CourseId);
                command.Parameters.AddWithValue("@ClassStartDate", newClass.ClassStartDate.Date);
                command.Parameters.AddWithValue("@ClassEndDate", newClass.ClassEndDate.Date);
                command.Parameters.AddWithValue("@Id", newClass.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Counts the total number of classes.
        /// </summary>
        /// <returns>The total number of classes.</returns>
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

        /// <summary>
        /// Inserts a new class.
        /// </summary>
        /// <param name="newClass">The class object to insert.</param>
        /// <returns>The ID of the newly inserted class.</returns>
        public int InsertClass(Class newClass)
        {
            var sql = """
                    INSERT INTO classes (classcode, courseid, classstartdate, classenddate) 
                    VALUES (@ClassCode, @CourseID, @ClassStartDate, @ClassEndDate)
                     returning id;
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassCode", newClass.ClassCode);
                command.Parameters.AddWithValue("@CourseID", newClass.CourseId);
                command.Parameters.AddWithValue("@ClassStartDate", newClass.ClassStartDate.Date);
                command.Parameters.AddWithValue("@ClassEndDate", newClass.ClassEndDate.Date);

                connection.Open();
                int id = (int)command.ExecuteScalar();
                newClass.Id = id;

                return id > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Gets the classes a student is enrolled in by the student's ID.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <returns>An observable collection of classes the student is enrolled in.</returns>
        public ObservableCollection<Class> GetEnrolledClassesByStudentId(int studentId)
        {
            var result = new ObservableCollection<Class>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                        SELECT c.id, c.courseid, c.classcode, c.classstartdate, c.classenddate
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
                                ClassStartDate = reader.GetDateTime(reader.GetOrdinal("classstartdate")),
                                ClassEndDate = reader.GetDateTime(reader.GetOrdinal("classenddate"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the classes a teacher is enrolled in by the teacher's ID.
        /// </summary>
        /// <param name="teacherId">The ID of the teacher.</param>
        /// <returns>An observable collection of classes the teacher is enrolled in.</returns>
        public ObservableCollection<Class> GetEnrolledClassesByTeacherId(int teacherId)
        {
            var result = new ObservableCollection<Class>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                        SELECT c.id, c.courseid, c.classcode, c.classstartdate, c.classenddate
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
