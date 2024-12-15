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
        public Submission GetSubmissionById(int id)
        {
            var result = new Submission();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT Id, AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade
                FROM Submissions
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result = new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
                            };
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

        public void SaveSubmission(Submission submission)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                INSERT INTO Submissions (AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade)
                VALUES (@AssignmentId, @UserId, @SubmissionDate, @FilePath, @FileName, @FileType, @Grade)
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", submission.AssignmentId);
                        command.Parameters.AddWithValue("@UserId", submission.UserId);
                        command.Parameters.AddWithValue("@SubmissionDate", submission.SubmissionDate);
                        command.Parameters.AddWithValue("@FilePath", submission.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", submission.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", submission.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Grade", submission.Grade ?? (object)DBNull.Value);

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

        public List<Submission> GetSubmissionsByAssignmentId(int id)
        {
            var result = new List<Submission>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, u.Username, s.Grade
                FROM Submissions s
                JOIN Users u ON s.UserId = u.Id
                WHERE s.AssignmentId=@AssignmentId
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
                            });
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

        public List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2)
        {
            var result = new List<Submission>();
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, s.Grade
                FROM Submissions s
                JOIN Users u ON s.UserId = u.Id
                WHERE s.AssignmentId=@AssignmentId AND s.UserId=@UserId
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", id1);
                        command.Parameters.AddWithValue("@UserId", id2);
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add(new Submission
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                SubmissionDate = reader.GetDateTime(reader.GetOrdinal("SubmissionDate")),
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                                FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Grade"))
                            });
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

        public void UpdateSubmission(Submission submission)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                UPDATE Submissions SET AssignmentId=@AssignmentId, UserId=@UserId, SubmissionDate=@SubmissionDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType, Grade=@Grade
                WHERE Id=@Id
                """;

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignmentId", submission.AssignmentId);
                        command.Parameters.AddWithValue("@UserId", submission.UserId);
                        command.Parameters.AddWithValue("@SubmissionDate", submission.SubmissionDate);
                        command.Parameters.AddWithValue("@FilePath", submission.FilePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileName", submission.FileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FileType", submission.FileType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Grade", submission.Grade ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", submission.Id);

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

        public void DeleteSubmissionById(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = "DELETE FROM Submissions WHERE Id=@Id";
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
    }
}
