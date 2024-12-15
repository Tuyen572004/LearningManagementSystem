using LearningManagementSystem.Helpers;
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
        public FullObservableCollection<BaseResource> findAssignmentsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, ClassId, ResourceCategoryId, Title, TeacherId, Description, DueDate, FilePath, FileName, FileType
            FROM Assignments
            WHERE ClassId=@ClassId
            """;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Assignment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        TeacherId = reader.GetInt32(reader.GetOrdinal("TeacherId")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                        FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                        FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType"))
                    });
                }
                this.CloseConnection();
            }
            return result;
        }

        public void SaveAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                INSERT INTO Assignments (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                VALUES (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }

        public void UpdateAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                UPDATE Assignments 
                SET ClassId=@ClassId, ResourceCategoryId=@ResourceCategoryId, Title=@Title, Description=@Description, TeacherId=@TeacherId, DueDate=@DueDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", assignment.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }


        public void DeleteAttachmentByAssignmentId(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                    UPDATE Assignments 
                    SET FileName=NULL, FilePath=NULL, FileType=NULL
                    WHERE Id=@AssignmentId
                    """;
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }

        public void AddAssignment(Assignment assignment)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                INSERT INTO Assignments (ClassId, ResourceCategoryId, Title, Description, TeacherId, DueDate, FilePath, FileName, FileType)
                VALUES (@ClassId, @ResourceCategoryId, @Title, @Description, @TeacherId, @DueDate, @FilePath, @FileName, @FileType)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClassId", assignment.ClassId);
                        command.Parameters.AddWithValue("@ResourceCategoryId", assignment.ResourceCategoryId);
                        command.Parameters.AddWithValue("@Title", assignment.Title);
                        command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TeacherId", assignment.TeacherId);
                        command.Parameters.AddWithValue("@DueDate", assignment.DueDate);
                        command.Parameters.AddWithValue("@FilePath", assignment.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", assignment.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", assignment.FileType ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }

        public void DeleteAssignmentById(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "DELETE FROM Assignments WHERE Id=@Id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
        }

        public Assignment GetAssignmentById(int assignmentId)
        {
            var result = new Assignment();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT Id, ClassId, ResourceCategoryId, Title, Description, DueDate, FilePath, FileName, FileType
                FROM Assignments
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", assignmentId);
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            result.ClassId = reader.GetInt32(reader.GetOrdinal("ClassId"));
                            result.ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId"));
                            result.Title = reader.GetString(reader.GetOrdinal("Title"));
                            result.Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"));
                            result.DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate"));
                            result.FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath"));
                            result.FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName"));
                            result.FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    this.CloseConnection();
                }
            }
            return result;
        }



    }
}
