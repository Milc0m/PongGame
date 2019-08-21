using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PongGame.Models;
using PongGame.Controllers;
using MySql.Data.MySqlClient;

namespace PongGame.Models
{
    public class LoginModel
    {




        public string GetUserInfo(string email)
        {
            string db = GetUserFromDB(email);
            return email + " " + db;
        }

        public string GetUserFromDB(string email)
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=test;pwd=test123;database=usersdb2";
            
            conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();
            string stm = "SELECT * FROM AspNetUsers WHERE UserName = " + email;
            MySqlCommand cmd = new MySqlCommand(stm, conn);
            string version = cmd.ExecuteScalar().ToString();
            conn.Close();
            return version;
        }
    }
}
