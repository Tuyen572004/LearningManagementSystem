using LearningManagementSystem.Services.ConfigService;
using Npgsql;

namespace LearningManagementSystem.DataAccess
{

    public partial class SqlDao : IDao
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDao"/> class.
        /// </summary>
        /// <param name="configService">The configuration service to retrieve database settings.</param>
        public SqlDao(IConfigService configService)
        {
            var config = configService.GetDatabaseConfig();
            _connectionString = $"Host={config.DB_HOST};Username={config.DB_USERNAME};Password={config.DB_PASSWORD};Database={config.DB_DATABASE};Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;";
        }

        /// <summary>
        /// Gets a new instance of the <see cref="NpgsqlConnection"/> class.
        /// </summary>
        /// <returns>A new <see cref="NpgsqlConnection"/> object.</returns>
        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        /// <summary>
        /// Gets the full command text with parameters replaced by their values.
        /// </summary>
        /// <param name="command">The <see cref="NpgsqlCommand"/> object.</param>
        /// <returns>The full command text with parameter values.</returns>
        public string GetFullCommandText(NpgsqlCommand command)
        {
            string commandText = command.CommandText;
            foreach (NpgsqlParameter param in command.Parameters)
            {
                commandText = commandText.Replace(param.ParameterName, param.Value.ToString());
            }
            return commandText;
        }
    }
    public class DatabaseConfig
    {
        /// <summary>
        /// Gets or sets the database host.
        /// </summary>
        public string DB_HOST { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DB_DATABASE { get; set; }

        /// <summary>
        /// Gets or sets the database username.
        /// </summary>
        public string DB_USERNAME { get; set; }

        /// <summary>
        /// Gets or sets the database password.
        /// </summary>
        public string DB_PASSWORD { get; set; }
    }
}