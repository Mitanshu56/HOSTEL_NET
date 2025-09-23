using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HostelManagement.Models
{
    public class RoomDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HostelDB"].ConnectionString;

        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Rooms ORDER BY RoomNumber";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    rooms.Add(new Room
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RoomNumber = reader["RoomNumber"].ToString(),
                        RoomType = reader["RoomType"].ToString(),
                        SharingType = Convert.ToInt32(reader["SharingType"]),
                        ACType = reader["ACType"].ToString(),
                        RentAmount = Convert.ToDecimal(reader["RentAmount"]),
                        FloorNumber = reader["FloorNumber"] != DBNull.Value ? Convert.ToInt32(reader["FloorNumber"]) : (int?)null,
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                        MaxOccupancy = Convert.ToInt32(reader["MaxOccupancy"]),
                        CurrentOccupancy = Convert.ToInt32(reader["CurrentOccupancy"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }
            return rooms;
        }

        public Room GetRoomById(int id)
        {
            Room room = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Rooms WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    room = new Room
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RoomNumber = reader["RoomNumber"].ToString(),
                        RoomType = reader["RoomType"].ToString(),
                        SharingType = Convert.ToInt32(reader["SharingType"]),
                        ACType = reader["ACType"].ToString(),
                        RentAmount = Convert.ToDecimal(reader["RentAmount"]),
                        FloorNumber = reader["FloorNumber"] != DBNull.Value ? Convert.ToInt32(reader["FloorNumber"]) : (int?)null,
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                        MaxOccupancy = Convert.ToInt32(reader["MaxOccupancy"]),
                        CurrentOccupancy = Convert.ToInt32(reader["CurrentOccupancy"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    };
                }
            }
            return room;
        }

        public List<Room> GetAvailableRooms()
        {
            List<Room> rooms = new List<Room>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Rooms WHERE CurrentOccupancy < MaxOccupancy ORDER BY RoomNumber";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    rooms.Add(new Room
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RoomNumber = reader["RoomNumber"].ToString(),
                        RoomType = reader["RoomType"].ToString(),
                        SharingType = Convert.ToInt32(reader["SharingType"]),
                        ACType = reader["ACType"].ToString(),
                        RentAmount = Convert.ToDecimal(reader["RentAmount"]),
                        MaxOccupancy = Convert.ToInt32(reader["MaxOccupancy"]),
                        CurrentOccupancy = Convert.ToInt32(reader["CurrentOccupancy"])
                    });
                }
            }
            return rooms;
        }

        public bool AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Rooms (RoomNumber, RoomType, SharingType, ACType, RentAmount, FloorNumber, MaxOccupancy, CreatedDate) 
                               VALUES (@RoomNumber, @RoomType, @SharingType, @ACType, @RentAmount, @FloorNumber, @MaxOccupancy, @CreatedDate)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
                cmd.Parameters.AddWithValue("@SharingType", room.SharingType);
                cmd.Parameters.AddWithValue("@ACType", room.ACType);
                cmd.Parameters.AddWithValue("@RentAmount", room.RentAmount);
                cmd.Parameters.AddWithValue("@FloorNumber", room.FloorNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxOccupancy", room.MaxOccupancy);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Rooms SET RoomNumber = @RoomNumber, RoomType = @RoomType, 
                               SharingType = @SharingType, ACType = @ACType, RentAmount = @RentAmount, 
                               FloorNumber = @FloorNumber, MaxOccupancy = @MaxOccupancy WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", room.Id);
                cmd.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                cmd.Parameters.AddWithValue("@RoomType", room.RoomType);
                cmd.Parameters.AddWithValue("@SharingType", room.SharingType);
                cmd.Parameters.AddWithValue("@ACType", room.ACType);
                cmd.Parameters.AddWithValue("@RentAmount", room.RentAmount);
                cmd.Parameters.AddWithValue("@FloorNumber", room.FloorNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxOccupancy", room.MaxOccupancy);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteRoom(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Check if room has students
                string checkQuery = "SELECT COUNT(*) FROM Students WHERE RoomId = @Id AND IsActive = 1";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                int studentCount = (int)checkCmd.ExecuteScalar();

                if (studentCount > 0)
                    return false; // Cannot delete room with active students

                string deleteQuery = "DELETE FROM Rooms WHERE Id = @Id";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@Id", id);

                return deleteCmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateRoomOccupancy(int roomId, int newOccupancy)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Rooms SET CurrentOccupancy = @Occupancy WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", roomId);
                cmd.Parameters.AddWithValue("@Occupancy", newOccupancy);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
