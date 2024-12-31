using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        /// <summary>
        /// Retrieves a submission by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the submission.</param>
        /// <returns>A <see cref="Submission"/> object if found; otherwise, a new <see cref="Submission"/> object.</returns>
        public Submission GetSubmissionById(int id)
        {
            var result = new Submission();
            var sql = """
                    SELECT Id, AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade
                    FROM Submissions
                    WHERE Id=@Id
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
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
            return result;
        }

        /// <summary>
        /// Saves a new submission to the database.
        /// </summary>
        /// <param name="submission">The <see cref="Submission"/> object to save.</param>
        public void SaveSubmission(Submission submission)
        {
            var sql = """
                    INSERT INTO Submissions (AssignmentId, UserId, SubmissionDate, FilePath, FileName, FileType, Grade)
                    VALUES (@AssignmentId, @UserId, @SubmissionDate, @FilePath, @FileName, @FileType, @Grade)
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@AssignmentId", submission.AssignmentId);
                command.Parameters.AddWithValue("@UserId", submission.UserId);
                command.Parameters.AddWithValue("@SubmissionDate", submission.SubmissionDate);
                command.Parameters.AddWithValue("@FilePath", submission.FilePath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileName", submission.FileName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileType", submission.FileType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Grade", submission.Grade ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves a list of submissions for a specific assignment.
        /// </summary>
        /// <param name="id">The unique identifier of the assignment.</param>
        /// <returns>A list of <see cref="Submission"/> objects.</returns>
        public List<Submission> GetSubmissionsByAssignmentId(int id)
        {
            var result = new List<Submission>();
            var sql = """
                    SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, u.Username, s.Grade
                    FROM Submissions s
                    JOIN Users u ON s.UserId = u.Id
                    WHERE s.AssignmentId=@AssignmentId
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@AssignmentId", id);
                connection.Open();
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
            return result;
        }

        /// <summary>
        /// Retrieves a list of submissions for a specific assignment and user.
        /// </summary>
        /// <param name="id1">The unique identifier of the assignment.</param>
        /// <param name="id2">The unique identifier of the user.</param>
        /// <returns>A list of <see cref="Submission"/> objects.</returns>
        public List<Submission> GetSubmissionsByAssignmentIdAndUserId(int id1, int id2)
        {
            var result = new List<Submission>();
            var sql = """
                    SELECT s.Id, s.AssignmentId, s.UserId, s.SubmissionDate, s.FilePath, s.FileName, s.FileType, s.Grade
                    FROM Submissions s
                    JOIN Users u ON s.UserId = u.Id
                    WHERE s.AssignmentId=@AssignmentId AND s.UserId=@UserId
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@AssignmentId", id1);
                command.Parameters.AddWithValue("@UserId", id2);
                connection.Open();
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
            return result;
        }

        /// <summary>
        /// Updates an existing submission in the database.
        /// </summary>
        /// <param name="submission">The <see cref="Submission"/> object to update.</param>
        public void UpdateSubmission(Submission submission)
        {
            var sql = """
                    UPDATE Submissions SET AssignmentId=@AssignmentId, UserId=@UserId, SubmissionDate=@SubmissionDate, FilePath=@FilePath, FileName=@FileName, FileType=@FileType, Grade=@Grade
                    WHERE Id=@Id
                    """;

            using (var connection = GetConnection())
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

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a submission by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the submission to delete.</param>
        public void DeleteSubmissionById(int id)
        {
            var sql = "DELETE FROM Submissions WHERE Id=@Id";

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
