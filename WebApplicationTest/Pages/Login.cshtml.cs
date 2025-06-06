using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTest.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "ID can not be empty!")]
        [Display(Name = "ID:")]
        public string inputID { set; get; }

        [BindProperty]
        [Required(ErrorMessage = "Password can not be empty!")]
        [MinLength(5, ErrorMessage = "Password must be at lesat 5 characters!")]
        public string inputPassword { set; get; }

        public string message;

        public readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public void OnPostLogin()
        {
            string connection = _configuration.GetConnectionString("SQLite");
            string query = "select * from [member] where [id] = @id and [password] = @password";

            SqliteConnection sqlConnection = new SqliteConnection(connection);
            SqliteCommand sqlCommand = new SqliteCommand(query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", inputID);
            sqlCommand.Parameters.AddWithValue("@password", inputPassword);
            sqlConnection.Open();
            SqliteDataReader sdr = sqlCommand.ExecuteReader();
            if (sdr.HasRows)
            {
                sdr.Read();
                string user_name = sdr.GetString(2);

                HttpContext.Session.SetString("user_id", inputID);
                HttpContext.Session.SetString("user_name", user_name);
                sqlConnection.Close();

                Response.Redirect("User");
            }
            else
            {
                message = "ID or Password error.";
                sqlConnection.Close();
            }
        }
    }
}
