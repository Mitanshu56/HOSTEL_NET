using System.Web;
using System.Web.Mvc;
using HostelManagement.Models;

namespace HostelManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAL userDal = new UserDAL();
        private readonly StudentDAL studentDal = new StudentDAL();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // Hard-coded admin
            if (username == "admin" && password == "admin123")
            {
                Session["Role"] = "Admin";
                Session["Username"] = username;
                return RedirectToAction("Index", "Home");
            }

            // Check Users table for others
            var user = userDal.GetByUsernameAndPassword(username, password);
            if (user != null)
            {
                Session["Role"] = "User";
                Session["Username"] = user.Username;
                Session["UserId"] = user.Id;
                Session["StudentId"] = user.StudentId;

                // If mapped to a student, go to Student Portal
                if (user.StudentId.HasValue)
                {
                    return RedirectToAction("Index", "StudentPortal");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
