using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManagement.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        [Required(ErrorMessage = "Sharing type is required")]
        [Display(Name = "Sharing")]
        [Range(1, 4, ErrorMessage = "Sharing must be between 1 and 4")]
        public int SharingType { get; set; }

        [Required(ErrorMessage = "AC type is required")]
        [Display(Name = "AC Type")]
        public string ACType { get; set; }

        [Required(ErrorMessage = "Rent amount is required")]
        [Display(Name = "Rent Amount (₹)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rent amount must be greater than 0")]
        public decimal RentAmount { get; set; }

        [Display(Name = "Floor Number")]
        public int? FloorNumber { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Max Occupancy")]
        public int MaxOccupancy { get; set; }

        [Display(Name = "Current Occupancy")]
        public int CurrentOccupancy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        // Computed properties
        public string SharingDisplay => $"{SharingType} Sharing";
        public bool HasVacancy => CurrentOccupancy < MaxOccupancy;
        public string AvailabilityStatus => HasVacancy ? "Available" : "Full";
    }
}
