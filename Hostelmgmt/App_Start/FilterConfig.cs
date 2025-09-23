using System.Web;
using System.Web.Mvc;
using HostelManagement.Filters;

namespace Hostelmgmt
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Add access control globally
            filters.Add(new AccessControlFilter());
        }
    }
}
