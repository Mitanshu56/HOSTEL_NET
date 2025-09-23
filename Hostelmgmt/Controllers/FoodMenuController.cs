using System.Web.Mvc;
using HostelManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HostelManagement.Controllers
{
    public class FoodMenuController : Controller
    {
        private FoodMenuDAL dal = new FoodMenuDAL();

        public ActionResult Index()
        {
            var menus = dal.GetAllFoodMenus();
            return View(menus);
        }

        public ActionResult WeeklyMenu()
        {
            var weeklyMenu = dal.GetWeeklyMenu();
            return View(weeklyMenu);
        }

        public ActionResult Details(int id)
        {
            var menu = dal.GetFoodMenuById(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        public ActionResult Create()
        {
            ViewBag.DaysOfWeek = GetDaysOfWeekList();
            ViewBag.MealTypes = GetMealTypesList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FoodMenu menu)
        {
            if (ModelState.IsValid)
            {
                // Check if menu already exists for this day and meal
                if (dal.MenuExistsForDayAndMeal(menu.DayOfWeek, menu.MealType))
                {
                    ModelState.AddModelError("", $"Menu for {menu.DayOfWeek} {menu.MealType} already exists. Please edit the existing menu.");
                    ViewBag.DaysOfWeek = GetDaysOfWeekList();
                    ViewBag.MealTypes = GetMealTypesList();
                    return View(menu);
                }

                menu.IsActive = true;
                menu.CreatedDate = DateTime.Now;
                menu.UpdatedDate = DateTime.Now;

                if (dal.AddFoodMenu(menu))
                {
                    TempData["Success"] = "Food menu added successfully!";
                    return RedirectToAction("WeeklyMenu");
                }
                ModelState.AddModelError("", "Error occurred while adding food menu.");
            }

            ViewBag.DaysOfWeek = GetDaysOfWeekList();
            ViewBag.MealTypes = GetMealTypesList();
            return View(menu);
        }

        public ActionResult Edit(int id)
        {
            var menu = dal.GetFoodMenuById(id);
            if (menu == null)
            {
                return HttpNotFound();
            }

            ViewBag.DaysOfWeek = GetDaysOfWeekList();
            ViewBag.MealTypes = GetMealTypesList();
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodMenu menu)
        {
            if (ModelState.IsValid)
            {
                // Check if menu already exists for this day and meal (excluding current menu)
                if (dal.MenuExistsForDayAndMeal(menu.DayOfWeek, menu.MealType, menu.Id))
                {
                    ModelState.AddModelError("", $"Menu for {menu.DayOfWeek} {menu.MealType} already exists. Please choose different day or meal type.");
                    ViewBag.DaysOfWeek = GetDaysOfWeekList();
                    ViewBag.MealTypes = GetMealTypesList();
                    return View(menu);
                }

                if (dal.UpdateFoodMenu(menu))
                {
                    TempData["Success"] = "Food menu updated successfully!";
                    return RedirectToAction("WeeklyMenu");
                }
                ModelState.AddModelError("", "Error occurred while updating food menu.");
            }

            ViewBag.DaysOfWeek = GetDaysOfWeekList();
            ViewBag.MealTypes = GetMealTypesList();
            return View(menu);
        }

        public ActionResult Delete(int id)
        {
            var menu = dal.GetFoodMenuById(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (dal.DeleteFoodMenu(id))
            {
                TempData["Success"] = "Food menu deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Error occurred while deleting food menu.";
            }
            return RedirectToAction("WeeklyMenu");
        }

        public ActionResult DayMenu(string day)
        {
            if (string.IsNullOrEmpty(day))
            {
                return RedirectToAction("WeeklyMenu");
            }

            var menus = dal.GetMenuByDay(day);
            ViewBag.SelectedDay = day;
            return View(menus);
        }

        private List<SelectListItem> GetDaysOfWeekList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Monday", Value = "Monday" },
                new SelectListItem { Text = "Tuesday", Value = "Tuesday" },
                new SelectListItem { Text = "Wednesday", Value = "Wednesday" },
                new SelectListItem { Text = "Thursday", Value = "Thursday" },
                new SelectListItem { Text = "Friday", Value = "Friday" },
                new SelectListItem { Text = "Saturday", Value = "Saturday" },
                new SelectListItem { Text = "Sunday", Value = "Sunday" }
            };
        }

        private List<SelectListItem> GetMealTypesList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Breakfast", Value = "Breakfast" },
                new SelectListItem { Text = "Lunch", Value = "Lunch" },
                new SelectListItem { Text = "Dinner", Value = "Dinner" }
            };
        }
    }
}
