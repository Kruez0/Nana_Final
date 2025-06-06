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
    public class UserEditModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "id can not be empty!")]
        [MinLength(5, ErrorMessage = "id must be at lesat 5 characters!")]
        public string user_id { set; get; }

        [BindProperty]
        [Required(ErrorMessage = "password can not be empty!")]
        //[DataType(DataType.Password)]
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

        public UserEditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet(string id)
        {
            user_sex = true;

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
            if (sdr.HasRows)
            {
                sdr.Read();
                user_id = sdr.GetString(0);
                user_password = sdr.GetString(1);
                user_name = sdr.GetString(2);
                user_age = sdr.GetInt32(3);
                user_sex = sdr.GetInt32(4) > 0 ? true : false;
                user_tel = sdr.GetString(5);
                user_address = sdr.GetString(6);
            }
            sdr.Close();
            sqlConnection.Close();
        }

        public void OnPostUpdate()
        {
            string connection = _configuration.GetConnectionString("SQLite");
            SqliteConnection sqlConnection = new SqliteConnection(connection);
            sqlConnection.Open();

            var transaction = sqlConnection.BeginTransaction();
            try
            {
                SqliteCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = @"UPDATE member SET password=@password, name=@name, age=@age, sex=@sex, tel=@tel, address=@address WHERE id = @id";
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
