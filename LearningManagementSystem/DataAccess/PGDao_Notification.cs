using LearningManagementSystem.Helpers;
using LearningManagementSystem.Models;
using Npgsql;
using System;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao
    {
        /// <summary>
        /// Finds notifications by class ID.
        /// </summary>
        /// <param name="classId">The ID of the class.</param>
        /// <returns>A collection of notifications for the specified class.</returns>
        FullObservableCollection<BaseResource> IDao.findNotificationsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            var sql = """
                    SELECT Id, ClassId, ResourceCategoryId, Description, PostDate, Title, FilePath, FileName, FileType, createdBy
                    FROM Notifications
                    WHERE ClassId=@ClassId
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ClassId", classId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Notification
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        PostDate = reader.GetDateTime(reader.GetOrdinal("PostDate")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                        FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                        FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                        CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"))
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Updates an existing notification.
        /// </summary>
        /// <param name="notification">The notification to update.</param>
        void IDao.UpdateNotification(Notification notification)
        {
            var sql = """
                    UPDATE Notifications 
                    SET Description=@Description, FileName=@FileName, FilePath=@FilePath, FileType=@FileType, PostDate=@PostDate,
                        Title=@Title, ClassId=@ClassId, ResourceCategoryId=@ResourceCategoryId, CreatedBy=@CreatedBy
                    WHERE Id=@Id
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Description", notification.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileName", notification.FileName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FilePath", notification.FilePath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileType", notification.FileType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PostDate", notification.PostDate);
                command.Parameters.AddWithValue("@Title", notification.Title);
                command.Parameters.AddWithValue("@ClassId", notification.ClassId);
                command.Parameters.AddWithValue("@ResourceCategoryId", notification.ResourceCategoryId);
                command.Parameters.AddWithValue("@CreatedBy", notification.CreatedBy);
                command.Parameters.AddWithValue("@Id", notification.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the attachment of a notification by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification.</param>
        void IDao.DeleteAttachmentByNotificationId(int id)
        {
            var sql = """
                    UPDATE Notifications 
                    SET FileName=NULL, FilePath=NULL, FileType=NULL
                    WHERE Id=@NotificationId
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NotificationId", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a new notification.
        /// </summary>
        /// <param name="notification">The notification to add.</param>
        void IDao.AddNotification(Notification notification)
        {
            var sql = """
                    INSERT INTO Notifications (Description, FileName, FilePath, FileType, PostDate, Title, ClassId, ResourceCategoryId, CreatedBy)
                    VALUES (@Description, @FileName, @FilePath, @FileType, @PostDate, @Title, @ClassId, @ResourceCategoryId, @CreatedBy)
                    RETURNING Id
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Description", notification.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileName", notification.FileName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FilePath", notification.FilePath ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileType", notification.FileType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PostDate", notification.PostDate);
                command.Parameters.AddWithValue("@Title", notification.Title);
                command.Parameters.AddWithValue("@ClassId", notification.ClassId);
                command.Parameters.AddWithValue("@ResourceCategoryId", notification.ResourceCategoryId);
                command.Parameters.AddWithValue("@CreatedBy", notification.CreatedBy);

                connection.Open();
                notification.Id = (int)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Finds a notification by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification.</param>
        /// <returns>The notification with the specified ID, or null if not found.</returns>
        Notification IDao.FindNotificationById(int id)
        {
            var sql = """
                    SELECT * FROM Notifications WHERE Id=@Id
                    """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Notification
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                            FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                            FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                            PostDate = reader.GetDateTime(reader.GetOrdinal("PostDate")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                            ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"))
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a notification by its ID.
        /// </summary>
        /// <param name="id">The ID of the notification.</param>
        void IDao.DeleteNotificationById(int id)
        {
            var sql = """
                    DELETE FROM Notifications WHERE Id=@Id
                    """;

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
