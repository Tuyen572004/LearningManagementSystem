using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningManagementSystem.Models
{
    public class DBConnection
    {
        private DBConnection()
        {

        }

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DBConnection _instance = null;

        public static DBConnection Instance()
        {
            _instance ??= new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            bool result = false;

            if (Connection == null)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;
                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, Username, Password);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
                result = true;
            }

            return result;
        }

        public void Close()
        {
            Connection.Close();
        }
    }
}


