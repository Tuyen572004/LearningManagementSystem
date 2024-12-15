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

            if (this.OpenConnection())
            {
                var sql = """
            SELECT Id, Name, Summary
            FROM ResourceCategories
            """;

                var command = new NpgsqlCommand(sql, connection);

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

                this.CloseConnection();
            }
            return result;
        }
    }
}
