using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace WebApplicationTest.Pages
{
    public class AboutUsModel : PageModel
    {
        public DataTable dataTable;
        public readonly IConfiguration _configuration;
        public void OnGet()
        {
            GetMemberInfo();
        }
        public AboutUsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private void GetMemberInfo()
        {
            string connection = _configuration.GetConnectionString("SQLite");
            string query = "select * from [member]";

            SqliteConnection sqlConnection = new SqliteConnection(connection);
            SqliteCommand sqlCommand = new SqliteCommand(query, sqlConnection);
            sqlConnection.Open();
            SqliteDataReader sdr = sqlCommand.ExecuteReader();
            dataTable = new DataTable();
            dataTable.Load(sdr);
            sdr.Close();
            sqlConnection.Close();
        }
    }
}
