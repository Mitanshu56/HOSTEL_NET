using System.Web.Mvc;
using HostelManagement.Models;
using System;
using System.Linq;

namespace HostelManagement.Controllers
{
    public class StudentController : Controller
    {
        private StudentDAL dal = new StudentDAL();
        private RoomDAL roomDAL = new RoomDAL();

        public ActionResult Index()
        {
            var students = dal.GetAllStudents();
            return View(students);
        }

        public ActionResult Details(int id)
        {
            var student = dal.GetStudentById(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        public ActionResult Create()
        {
            ViewBag.AvailableRooms = GetAvailableRoomsList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                student.DateOfJoining = DateTime.Now;
                if (dal.AddStudent(student))
                {
                    TempData["Success"] = "Student added successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error occurred while adding student.");
            }

            ViewBag.AvailableRooms = GetAvailableRoomsList();
            return View(student);
        }

        public ActionResult Edit(int id)
        {
            var student = dal.GetStudentById(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            ViewBag.AvailableRooms = GetAvailableRoomsList(student.RoomId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                if (dal.UpdateStudent(student))
                {
                    TempData["Success"] = "Student updated successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error occurred while updating student.");
            }

            ViewBag.AvailableRooms = GetAvailableRoomsList(student.RoomId);
            return View(student);
        }

        public ActionResult Delete(int id)
        {
            var student = dal.GetStudentById(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (dal.DeleteStudent(id))
            {
                TempData["Success"] = "Student removed successfully!";
            }
            else
            {
                TempData["Error"] = "Error occurred while removing student.";
            }
            return RedirectToAction("Index");
        }

        private SelectList GetAvailableRoomsList(int? selectedRoomId = null)
        {
            var availableRooms = roomDAL.GetAvailableRooms();
            var roomList = availableRooms.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"{r.RoomNumber} - {r.SharingType} Sharing, {r.ACType} (₹{r.RentAmount:N0}) - {r.MaxOccupancy - r.CurrentOccupancy} slots available"
            }).ToList();

            roomList.Insert(0, new SelectListItem { Text = "-- Select Room --", Value = "" });

            return new SelectList(roomList, "Value", "Text", selectedRoomId);
        }
    }
}
