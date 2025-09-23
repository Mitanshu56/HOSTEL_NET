using System.Web.Mvc;
using HostelManagement.Models;
using System;
using System.Collections.Generic;

namespace HostelManagement.Controllers
{
    public class RoomController : Controller
    {
        private RoomDAL dal = new RoomDAL();

        public ActionResult Index()
        {
            var rooms = dal.GetAllRooms();
            return View(rooms);
        }

        public ActionResult Details(int id)
        {
            var room = dal.GetRoomById(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        public ActionResult Create()
        {
            ViewBag.RoomTypes = GetRoomTypeList();
            ViewBag.SharingTypes = GetSharingTypeList();
            ViewBag.ACTypes = GetACTypeList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Room room)
        {
            if (ModelState.IsValid)
            {
                room.MaxOccupancy = room.SharingType;
                room.CreatedDate = DateTime.Now;

                if (dal.AddRoom(room))
                {
                    TempData["Success"] = "Room added successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error occurred while adding room.");
            }

            ViewBag.RoomTypes = GetRoomTypeList();
            ViewBag.SharingTypes = GetSharingTypeList();
            ViewBag.ACTypes = GetACTypeList();
            return View(room);
        }

        public ActionResult Edit(int id)
        {
            var room = dal.GetRoomById(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            ViewBag.RoomTypes = GetRoomTypeList();
            ViewBag.SharingTypes = GetSharingTypeList();
            ViewBag.ACTypes = GetACTypeList();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Room room)
        {
            if (ModelState.IsValid)
            {
                room.MaxOccupancy = room.SharingType;

                if (dal.UpdateRoom(room))
                {
                    TempData["Success"] = "Room updated successfully!";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error occurred while updating room.");
            }

            ViewBag.RoomTypes = GetRoomTypeList();
            ViewBag.SharingTypes = GetSharingTypeList();
            ViewBag.ACTypes = GetACTypeList();
            return View(room);
        }

        public ActionResult Delete(int id)
        {
            var room = dal.GetRoomById(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (dal.DeleteRoom(id))
            {
                TempData["Success"] = "Room deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Cannot delete room. It may have active students assigned.";
            }
            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetRoomTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Single", Value = "Single" },
                new SelectListItem { Text = "Double", Value = "Double" },
                new SelectListItem { Text = "Triple", Value = "Triple" },
                new SelectListItem { Text = "Quad", Value = "Quad" }
            };
        }

        private List<SelectListItem> GetSharingTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "1 Sharing", Value = "1" },
                new SelectListItem { Text = "2 Sharing", Value = "2" },
                new SelectListItem { Text = "3 Sharing", Value = "3" },
                new SelectListItem { Text = "4 Sharing", Value = "4" }
            };
        }

        private List<SelectListItem> GetACTypeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "AC", Value = "AC" },
                new SelectListItem { Text = "Non-AC", Value = "Non-AC" }
            };
        }
    }
}
