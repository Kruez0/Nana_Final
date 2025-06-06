using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTest.Pages
{
    public class UserModel : PageModel
    {
        public string userID;
        public string userName;

        public DataTable dataTable;

        public readonly IConfiguration _configuration;

        public UserModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            userID = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(userID))
            {
                // Redirect to the login page if not logged in
                return RedirectToPage("Login");
            }

            // Continue normally if logged in
            GetMemberInfo();

            return Page();
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

        public void OnPostLogout()
        {
            HttpContext.Session.Remove("user_id");
            HttpContext.Session.Remove("user_name");
            Response.Redirect("Index");
        }

        public void OnPostDelete(string id)
        {
            string connection = _configuration.GetConnectionString("SQLite");
            SqliteConnection sqlConnection = new SqliteConnection(connection);
            sqlConnection.Open();

            var transaction = sqlConnection.BeginTransaction();
            try
            {
                SqliteCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = @"DELETE FROM member WHERE id = @id";
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.ExecuteNonQuery();

                transaction.Commit();

                sqlConnection.Close();

                Response.Redirect("User");
            }
            catch
            {
                transaction.Rollback();

                sqlConnection.Close();
            }
        }
    }
}
