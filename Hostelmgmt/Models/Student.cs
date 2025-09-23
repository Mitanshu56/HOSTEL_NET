using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Student Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Room")]
        public int? RoomId { get; set; }

        public string Address { get; set; }

        [Display(Name = "Date of Joining")]
        public DateTime DateOfJoining { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        // Navigation properties
        public string RoomNumber { get; set; }
        public string RoomDetails { get; set; }
    }
}
