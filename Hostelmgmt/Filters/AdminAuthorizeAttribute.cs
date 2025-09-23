using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HostelManagement.Filters
{
    // Restricts access to Admin only based on Session["Role"]
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var role = httpContext?.Session?["Role"] as string;
            return string.Equals(role, "Admin");
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            var username = session?["Username"] as string;
            var studentId = session?["StudentId"];

            if (!string.IsNullOrEmpty(username) && studentId != null)
            {
                // Logged in as student: send to their portal
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "StudentPortal",
                    action = "Index"
                }));
            }
            else
            {
                // Not logged in: send to login
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login"
                }));
            }
        }
    }
}
