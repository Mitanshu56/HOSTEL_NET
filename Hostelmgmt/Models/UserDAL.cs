using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HostelManagement.Models
{
    public class UserDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HostelDB"].ConnectionString;

        public User GetByUsernameAndPassword(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 1 Id, Username, Password, StudentId, IsActive, CreatedDate
                                   FROM Users 
                                   WHERE Username = @Username AND Password = @Password AND IsActive = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        StudentId = reader["StudentId"] != DBNull.Value ? Convert.ToInt32(reader["StudentId"]) : (int?)null,
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                }
            }
            return null;
        }
    }
}
