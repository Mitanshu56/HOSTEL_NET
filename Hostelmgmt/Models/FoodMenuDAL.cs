using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HostelManagement.Models
{
    public class FoodMenuDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HostelDB"].ConnectionString;

        public List<FoodMenu> GetAllFoodMenus()
        {
            List<FoodMenu> menus = new List<FoodMenu>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM FoodMenus WHERE IsActive = 1 
                               ORDER BY 
                               CASE DayOfWeek 
                                   WHEN 'Monday' THEN 1
                                   WHEN 'Tuesday' THEN 2
                                   WHEN 'Wednesday' THEN 3
                                   WHEN 'Thursday' THEN 4
                                   WHEN 'Friday' THEN 5
                                   WHEN 'Saturday' THEN 6
                                   WHEN 'Sunday' THEN 7
                               END,
                               CASE MealType
                                   WHEN 'Breakfast' THEN 1
                                   WHEN 'Lunch' THEN 2
                                   WHEN 'Dinner' THEN 3
                               END";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    menus.Add(new FoodMenu
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        DayOfWeek = reader["DayOfWeek"].ToString(),
                        MealType = reader["MealType"].ToString(),
                        MenuItems = reader["MenuItems"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"])
                    });
                }
            }
            return menus;
        }

        public List<FoodMenu> GetMenuByDay(string dayOfWeek)
        {
            List<FoodMenu> menus = new List<FoodMenu>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM FoodMenus WHERE DayOfWeek = @DayOfWeek AND IsActive = 1 
                               ORDER BY CASE MealType
                                   WHEN 'Breakfast' THEN 1
                                   WHEN 'Lunch' THEN 2
                                   WHEN 'Dinner' THEN 3
                               END";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    menus.Add(new FoodMenu
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        DayOfWeek = reader["DayOfWeek"].ToString(),
                        MealType = reader["MealType"].ToString(),
                        MenuItems = reader["MenuItems"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"])
                    });
                }
            }
            return menus;
        }

        public FoodMenu GetFoodMenuById(int id)
        {
            FoodMenu menu = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM FoodMenus WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    menu = new FoodMenu
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        DayOfWeek = reader["DayOfWeek"].ToString(),
                        MealType = reader["MealType"].ToString(),
                        MenuItems = reader["MenuItems"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"])
                    };
                }
            }
            return menu;
        }

        public bool AddFoodMenu(FoodMenu menu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO FoodMenus (DayOfWeek, MealType, MenuItems, Description, IsActive, CreatedDate, UpdatedDate) 
                               VALUES (@DayOfWeek, @MealType, @MenuItems, @Description, @IsActive, @CreatedDate, @UpdatedDate)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DayOfWeek", menu.DayOfWeek);
                cmd.Parameters.AddWithValue("@MealType", menu.MealType);
                cmd.Parameters.AddWithValue("@MenuItems", menu.MenuItems);
                cmd.Parameters.AddWithValue("@Description", menu.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", menu.IsActive);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateFoodMenu(FoodMenu menu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE FoodMenus SET DayOfWeek = @DayOfWeek, MealType = @MealType, 
                               MenuItems = @MenuItems, Description = @Description, IsActive = @IsActive, 
                               UpdatedDate = @UpdatedDate WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", menu.Id);
                cmd.Parameters.AddWithValue("@DayOfWeek", menu.DayOfWeek);
                cmd.Parameters.AddWithValue("@MealType", menu.MealType);
                cmd.Parameters.AddWithValue("@MenuItems", menu.MenuItems);
                cmd.Parameters.AddWithValue("@Description", menu.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", menu.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteFoodMenu(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE FoodMenus SET IsActive = 0, UpdatedDate = @UpdatedDate WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public Dictionary<string, List<FoodMenu>> GetWeeklyMenu()
        {
            var weeklyMenu = new Dictionary<string, List<FoodMenu>>();
            var allMenus = GetAllFoodMenus();

            var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            foreach (var day in daysOfWeek)
            {
                weeklyMenu[day] = allMenus.Where(m => m.DayOfWeek == day).ToList();
            }

            return weeklyMenu;
        }

        public bool MenuExistsForDayAndMeal(string dayOfWeek, string mealType, int? excludeId = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM FoodMenus WHERE DayOfWeek = @DayOfWeek AND MealType = @MealType AND IsActive = 1";
                if (excludeId.HasValue)
                {
                    query += " AND Id != @ExcludeId";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                cmd.Parameters.AddWithValue("@MealType", mealType);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                }

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
