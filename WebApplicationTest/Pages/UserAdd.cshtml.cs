using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace WebApplicationTest.Pages
{
    public class UserAddModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "id can not be empty!")]
        [MinLength(5, ErrorMessage = "id must be at lesat 5 characters!")]
        public string user_id { set; get; }

        [BindProperty]
        [Required(ErrorMessage = "password can not be empty!")]
        [DataType(DataType.Password)]
        public string user_password { set; get; }

        [BindProperty]
        [Required(ErrorMessage = "name can not be empty!")]
        public string user_name { set; get; }

        [BindProperty]
        public int? user_age { set; get; }

        [BindProperty]
        public bool user_sex { set; get; }

        [BindProperty]
        public string user_tel { set; get; }

        [BindProperty]
        public string user_address { set; get; }

        [BindProperty]
        public string message { set; get; }

        public readonly IConfiguration _configuration;

        public UserAddModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            user_sex = true;
        }

        public void OnPostAdd()
        {
            string connection = _configuration.GetConnectionString("SQLite");
            //string query = @"INSERT INTO member (id, password, name, age, sex, tel, address) VALUES (@id, @password, @name, @age, @sex, @tel, @address)";

            SqliteConnection sqlConnection = new SqliteConnection(connection);
            sqlConnection.Open();

            var transaction = sqlConnection.BeginTransaction();
            try
            {
                //SqliteCommand sqlCommand = new SqliteCommand(query, sqlConnection);
                SqliteCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = @"INSERT INTO member (id, password, name, age, sex, tel, address) VALUES (@id, @password, @name, @age, @sex, @tel, @address)";
                sqlCommand.Parameters.AddWithValue("@id", user_id);
                sqlCommand.Parameters.AddWithValue("@password", user_password);
                sqlCommand.Parameters.AddWithValue("@name", user_name);
                sqlCommand.Parameters.AddWithValue("@age", user_age == null ? DBNull.Value : user_age);
                sqlCommand.Parameters.AddWithValue("@sex", user_sex);
                sqlCommand.Parameters.AddWithValue("@tel", user_tel == null ? DBNull.Value : user_tel);
                sqlCommand.Parameters.AddWithValue("@address", user_tel == null ? DBNull.Value : user_address);
                sqlCommand.ExecuteNonQuery();

                transaction.Commit();

                sqlConnection.Close();

                Response.Redirect("User");
            }
            catch
            {
                transaction.Rollback();

                sqlConnection.Close();

                message = "Faild to insert";
            }
        }

        public void OnPostCancel()
        {
            Response.Redirect("User");
        }
    }
}
