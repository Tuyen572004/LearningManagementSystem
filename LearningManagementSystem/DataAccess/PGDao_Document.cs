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
        public FullObservableCollection<BaseResource> findDocumentsByClassId(int classId)
        {
            var result = new FullObservableCollection<BaseResource>();
            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, ClassId, ResourceCategoryId, FileName, FilePath, FileType, UploadDate, Title
            FROM Documents
            WHERE ClassId=@ClassId
            """;
                var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ClassId", classId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Document
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                        ResourceCategoryId = reader.GetInt32(reader.GetOrdinal("ResourceCategoryId")),
                        FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                        FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                        FileType = reader.IsDBNull(reader.GetOrdinal("FileType")) ? null : reader.GetString(reader.GetOrdinal("FileType")),
                        UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                        Title = reader.GetString(reader.GetOrdinal("Title"))
                    });
                }
                this.CloseConnection();
            }
            return result;
        }
    }
}
