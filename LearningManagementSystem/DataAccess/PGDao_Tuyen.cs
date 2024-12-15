using LearningManagementSystem.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.DataAccess
{
    public partial class SqlDao : IDao
    {
        void IDao.UpdateNotification(Notification notification)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    UPDATE Notifications 
                    SET Description=@Description, FileName=@FileName, FilePath=@FilePath, FileType=@FileType, PostDate=@PostDate,
                        Title=@Title, ClassId=@ClassId, ResourceCategoryId=@ResourceCategoryId, CreatedBy=@CreatedBy
                    WHERE Id=@Id
                    """;

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

                    command.ExecuteNonQuery();
                }
                this.CloseConnection();
            }
        }

        void IDao.DeleteAttachmentByNotificationId(int id)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    UPDATE Notifications 
                    SET FileName=NULL, FilePath=NULL, FileType=NULL
                    WHERE Id=@NotificationId
                    """;
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NotificationId", id);
                    command.ExecuteNonQuery();
                }
                this.CloseConnection();
            }
        }

        void IDao.AddNotification(Notification notification)
        {
            try{
                if (this.OpenConnection())
                {
                    var sql = """
                        INSERT INTO Notifications (Description, FileName, FilePath, FileType, PostDate, Title, ClassId, ResourceCategoryId, CreatedBy)
                        VALUES (@Description, @FileName, @FilePath, @FileType, @PostDate, @Title, @ClassId, @ResourceCategoryId, @CreatedBy)
                        RETURNING Id
                        """;

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

                        notification.Id = (int)command.ExecuteScalar();
                    }
                    this.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                this.CloseConnection();
                throw ex;
            }
        }

        Notification IDao.FindNotificationById(int id)
        {
            if (this.OpenConnection())
            {
                var sql = """
                    SELECT * FROM Notifications WHERE Id=@Id
                    """;
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
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
                this.CloseConnection();
            }
            return null;
        }

        void IDao.DeleteNotificationById(int id)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var sql = """
                    DELETE FROM Notifications WHERE Id=@Id
                    """;
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    this.CloseConnection();
                }
            }
            catch(Exception ex)
            {
                this.CloseConnection();
            }
        }

    }
}
