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
        public List<ResourceCategory> findAllResourceCategories()
        {
            var result = new List<ResourceCategory>();
            var sql = """
                SELECT Id, Name, Summary
                FROM ResourceCategories
                """;

            using (var connection = GetConnection())
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new ResourceCategory
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Summary = reader.GetString(reader.GetOrdinal("Summary"))
                    });
                }
            }
            return result;
        }
    }
}
