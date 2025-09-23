using System.Web.Mvc;
using HostelManagement.Models;

namespace HostelManagement.Controllers
{
    public class StudentPortalController : Controller
    {
        private readonly StudentDAL studentDal = new StudentDAL();
        private readonly RoomDAL roomDal = new RoomDAL();

        // GET: /portal
        public ActionResult Index()
        {
            if (Session["StudentId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Safely convert to int (Session may hold boxed int? or string)
            var studentId = System.Convert.ToInt32(Session["StudentId"]);

            var student = studentDal.GetStudentById(studentId);
            if (student == null)
            {
                // Safety: student no longer exists
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            // Ensure room details are populated consistently
            if (student.RoomId.HasValue)
            {
                var room = roomDal.GetRoomById(student.RoomId.Value);
                if (room != null)
                {
                    student.RoomNumber = room.RoomNumber;
                    student.RoomDetails = string.Format("{0} ({1} Sharing, {2})", room.RoomNumber, room.SharingType, room.ACType);
                    ViewBag.Room = room;
                }
            }

            return View(student);
        }
    }
}
