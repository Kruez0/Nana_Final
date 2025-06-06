using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTest.Pages
{
    public class UserInfoModel : PageModel
    {
        public string user_id;

        public DataTable dataTable;

        public readonly IConfiguration _configuration;

        public UserInfoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet(string id)
        {
            GetMemberInfo(id);
        }

        private void GetMemberInfo(string id)
        {
            string connection = _configuration.GetConnectionString("SQLite");
            string query = "select * from [member] where id = @id";

            SqliteConnection sqlConnection = new SqliteConnection(connection);
            SqliteCommand sqlCommand = new SqliteCommand(query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlConnection.Open();
            SqliteDataReader sdr = sqlCommand.ExecuteReader();
            dataTable = new DataTable();
            dataTable.Load(sdr);
            sdr.Close();
            sqlConnection.Close();
        }
    }
}
