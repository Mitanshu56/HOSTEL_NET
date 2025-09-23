using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HostelManagement.Models
{
    public class StudentDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["HostelDB"].ConnectionString;
        private RoomDAL roomDAL = new RoomDAL();

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT s.*, r.RoomNumber, 
                               CONCAT(r.RoomNumber, ' (', r.SharingType, ' Sharing, ', r.ACType, ')') as RoomDetails
                               FROM Students s 
                               LEFT JOIN Rooms r ON s.RoomId = r.Id 
                               WHERE s.IsActive = 1
                               ORDER BY s.DateOfJoining DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        RoomId = reader["RoomId"] != DBNull.Value ? Convert.ToInt32(reader["RoomId"]) : (int?)null,
                        Address = reader["Address"].ToString(),
                        DateOfJoining = Convert.ToDateTime(reader["DateOfJoining"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        RoomNumber = reader["RoomNumber"]?.ToString(),
                        RoomDetails = reader["RoomDetails"]?.ToString()
                    });
                }
            }
            return students;
        }

        public Student GetStudentById(int id)
        {
            Student student = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT s.*, r.RoomNumber, 
                               CONCAT(r.RoomNumber, ' (', r.SharingType, ' Sharing, ', r.ACType, ')') as RoomDetails
                               FROM Students s 
                               LEFT JOIN Rooms r ON s.RoomId = r.Id 
                               WHERE s.Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    student = new Student
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        RoomId = reader["RoomId"] != DBNull.Value ? Convert.ToInt32(reader["RoomId"]) : (int?)null,
                        Address = reader["Address"].ToString(),
                        DateOfJoining = Convert.ToDateTime(reader["DateOfJoining"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        RoomNumber = reader["RoomNumber"]?.ToString(),
                        RoomDetails = reader["RoomDetails"]?.ToString()
                    };
                }
            }
            return student;
        }

        public bool AddStudent(Student student)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string query = @"INSERT INTO Students (Name, Email, Phone, RoomId, Address, DateOfJoining, IsActive) 
                                   VALUES (@Name, @Email, @Phone, @RoomId, @Address, @DateOfJoining, @IsActive)";
                    SqlCommand cmd = new SqlCommand(query, conn, transaction);
                    cmd.Parameters.AddWithValue("@Name", student.Name);
                    cmd.Parameters.AddWithValue("@Email", student.Email);
                    cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoomId", student.RoomId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfJoining", student.DateOfJoining);
                    cmd.Parameters.AddWithValue("@IsActive", true);

                    bool result = cmd.ExecuteNonQuery() > 0;

                    // Update room occupancy if room is assigned
                    if (result && student.RoomId.HasValue)
                    {
                        UpdateRoomOccupancyInTransaction(student.RoomId.Value, conn, transaction);
                    }

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool UpdateStudent(Student student)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Get old room ID
                    string getOldRoomQuery = "SELECT RoomId FROM Students WHERE Id = @Id";
                    SqlCommand getOldRoomCmd = new SqlCommand(getOldRoomQuery, conn, transaction);
                    getOldRoomCmd.Parameters.AddWithValue("@Id", student.Id);
                    var oldRoomId = getOldRoomCmd.ExecuteScalar();

                    string query = @"UPDATE Students SET Name = @Name, Email = @Email, Phone = @Phone, 
                                   RoomId = @RoomId, Address = @Address WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn, transaction);
                    cmd.Parameters.AddWithValue("@Id", student.Id);
                    cmd.Parameters.AddWithValue("@Name", student.Name);
                    cmd.Parameters.AddWithValue("@Email", student.Email);
                    cmd.Parameters.AddWithValue("@Phone", student.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoomId", student.RoomId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", student.Address ?? (object)DBNull.Value);

                    bool result = cmd.ExecuteNonQuery() > 0;

                    // Update room occupancies
                    if (result)
                    {
                        // Decrease old room occupancy
                        if (oldRoomId != null && oldRoomId != DBNull.Value)
                        {
                            UpdateRoomOccupancyInTransaction(Convert.ToInt32(oldRoomId), conn, transaction);
                        }

                        // Increase new room occupancy
                        if (student.RoomId.HasValue)
                        {
                            UpdateRoomOccupancyInTransaction(student.RoomId.Value, conn, transaction);
                        }
                    }

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool DeleteStudent(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Get room ID before deletion
                    string getRoomQuery = "SELECT RoomId FROM Students WHERE Id = @Id";
                    SqlCommand getRoomCmd = new SqlCommand(getRoomQuery, conn, transaction);
                    getRoomCmd.Parameters.AddWithValue("@Id", id);
                    var roomId = getRoomCmd.ExecuteScalar();

                    string query = "UPDATE Students SET IsActive = 0 WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn, transaction);
                    cmd.Parameters.AddWithValue("@Id", id);

                    bool result = cmd.ExecuteNonQuery() > 0;

                    // Update room occupancy
                    if (result && roomId != null && roomId != DBNull.Value)
                    {
                        UpdateRoomOccupancyInTransaction(Convert.ToInt32(roomId), conn, transaction);
                    }

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        private void UpdateRoomOccupancyInTransaction(int roomId, SqlConnection conn, SqlTransaction transaction)
        {
            string countQuery = "SELECT COUNT(*) FROM Students WHERE RoomId = @RoomId AND IsActive = 1";
            SqlCommand countCmd = new SqlCommand(countQuery, conn, transaction);
            countCmd.Parameters.AddWithValue("@RoomId", roomId);
            int currentOccupancy = (int)countCmd.ExecuteScalar();

            string updateQuery = "UPDATE Rooms SET CurrentOccupancy = @Occupancy WHERE Id = @Id";
            SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction);
            updateCmd.Parameters.AddWithValue("@Id", roomId);
            updateCmd.Parameters.AddWithValue("@Occupancy", currentOccupancy);
            updateCmd.ExecuteNonQuery();
        }
    }
}
