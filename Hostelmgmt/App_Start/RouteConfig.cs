using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HostelManagement
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Food Menu specific routes
            routes.MapRoute(
                name: "WeeklyFoodMenu",
                url: "foodmenu/weekly",
                defaults: new { controller = "FoodMenu", action = "WeeklyMenu" }
            );

            routes.MapRoute(
                name: "DayFoodMenu",
                url: "foodmenu/day/{day}",
                defaults: new { controller = "FoodMenu", action = "DayMenu" },
                constraints: new { day = @"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$" }
            );

            routes.MapRoute(
                name: "FoodMenuCreate",
                url: "foodmenu/add",
                defaults: new { controller = "FoodMenu", action = "Create" }
            );

            routes.MapRoute(
                name: "FoodMenuEdit",
                url: "foodmenu/edit/{id}",
                defaults: new { controller = "FoodMenu", action = "Edit" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "FoodMenuDelete",
                url: "foodmenu/delete/{id}",
                defaults: new { controller = "FoodMenu", action = "Delete" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "FoodMenuDetails",
                url: "foodmenu/details/{id}",
                defaults: new { controller = "FoodMenu", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "FoodMenuManagement",
                url: "foodmenu/{action}/{id}",
                defaults: new { controller = "FoodMenu", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            // Student management routes
            routes.MapRoute(
                name: "StudentCreate",
                url: "students/add",
                defaults: new { controller = "Student", action = "Create" }
            );

            routes.MapRoute(
                name: "StudentEdit",
                url: "students/edit/{id}",
                defaults: new { controller = "Student", action = "Edit" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "StudentDelete",
                url: "students/delete/{id}",
                defaults: new { controller = "Student", action = "Delete" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "StudentDetails",
                url: "students/details/{id}",
                defaults: new { controller = "Student", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "StudentsList",
                url: "students",
                defaults: new { controller = "Student", action = "Index" }
            );

            routes.MapRoute(
                name: "StudentManagement",
                url: "students/{action}/{id}",
                defaults: new { controller = "Student", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            // Room management routes
            routes.MapRoute(
                name: "RoomCreate",
                url: "rooms/add",
                defaults: new { controller = "Room", action = "Create" }
            );

            routes.MapRoute(
                name: "RoomEdit",
                url: "rooms/edit/{id}",
                defaults: new { controller = "Room", action = "Edit" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "RoomDelete",
                url: "rooms/delete/{id}",
                defaults: new { controller = "Room", action = "Delete" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "RoomDetails",
                url: "rooms/details/{id}",
                defaults: new { controller = "Room", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "RoomsList",
                url: "rooms",
                defaults: new { controller = "Room", action = "Index" }
            );

            routes.MapRoute(
                name: "RoomManagement",
                url: "rooms/{action}/{id}",
                defaults: new { controller = "Room", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            // Home routes
            routes.MapRoute(
                name: "Dashboard",
                url: "dashboard",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "About",
                url: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "Contact",
                url: "contact",
                defaults: new { controller = "Home", action = "Contact" }
            );

            routes.MapRoute(
                name: "Home",
                url: "home/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );

            // API-style routes for future AJAX calls
            routes.MapRoute(
                name: "ApiStudents",
                url: "api/students/{action}/{id}",
                defaults: new { controller = "Student", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            routes.MapRoute(
                name: "ApiRooms",
                url: "api/rooms/{action}/{id}",
                defaults: new { controller = "Room", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            routes.MapRoute(
                name: "ApiFoodMenu",
                url: "api/foodmenu/{action}/{id}",
                defaults: new { controller = "FoodMenu", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            // Catch-all route for better SEO and user experience
            routes.MapRoute(
                name: "CatchAll",
                url: "{category}/{subcategory}",
                defaults: new { controller = "Home", action = "Index" },
                constraints: new { category = @"^(admin|manage|system)$" }
            );

            // Root route (show login first)
            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "Account", action = "Login" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            // Pretty login URL
            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            // Student Portal
            routes.MapRoute(
                name: "StudentPortal",
                url: "portal",
                defaults: new { controller = "StudentPortal", action = "Index" },
                namespaces: new[] { "HostelManagement.Controllers" }
            );

            // Default route (keep this last)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "HostelManagement.Controllers" }
            );
        }
    }
}
