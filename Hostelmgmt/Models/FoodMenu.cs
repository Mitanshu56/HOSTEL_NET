using System;
using System.ComponentModel.DataAnnotations;

namespace HostelManagement.Models
{
    public class FoodMenu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Day of week is required")]
        [Display(Name = "Day of Week")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "Meal type is required")]
        [Display(Name = "Meal Type")]
        public string MealType { get; set; }

        [Required(ErrorMessage = "Menu items are required")]
        [Display(Name = "Menu Items")]
        [StringLength(500, ErrorMessage = "Menu items cannot exceed 500 characters")]
        public string MenuItems { get; set; }

        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }

        // Computed properties
        public string MealIcon
        {
            get
            {
                switch (MealType)
                {
                    case "Breakfast":
                        return "fas fa-coffee";
                    case "Lunch":
                        return "fas fa-utensils";
                    case "Dinner":
                        return "fas fa-moon";
                    default:
                        return "fas fa-cutlery";
                }
            }
        }

        public string MealColor
        {
            get
            {
                switch (MealType)
                {
                    case "Breakfast":
                        return "warning";
                    case "Lunch":
                        return "success";
                    case "Dinner":
                        return "info";
                    default:
                        return "secondary";
                }
            }
        }
    }
}
