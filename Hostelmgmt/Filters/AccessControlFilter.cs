using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HostelManagement.Filters
{
    // Global access control to restrict students to their portal only
    public class AccessControlFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var rd = filterContext.RouteData;
            var controller = (rd.Values["controller"] ?? string.Empty).ToString();
            var action = (rd.Values["action"] ?? string.Empty).ToString();

            var session = filterContext.HttpContext.Session;
            var role = session != null ? session["Role"] as string : null;
            var hasStudentId = session != null && session["StudentId"] != null;

            bool isAdmin = string.Equals(role, "Admin");
            bool isAccount = controller == "Account";
            bool isStudentPortal = controller == "StudentPortal";

            // Admin has full access
            if (isAdmin)
            {
                return;
            }

            // Student logged-in: allow only StudentPortal and Account (Login/Logout)
            if (hasStudentId)
            {
                if (!(isStudentPortal || isAccount))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "StudentPortal", action = "Index" }));
                }
                return;
            }

            // Anonymous: allow only Account/Login
            if (!isAccount)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
        }
    }
}
