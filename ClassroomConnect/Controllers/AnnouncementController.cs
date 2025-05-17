using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    public class AnnouncementController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Post([Bind("ContentHtml,ClassId")] Announcement announcement)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _unitOfWork.Classes.Get(c => c.Id == announcement.ClassId);

            if (@class == null) return Json(new { success = false, message = "Class not found." });

            announcement.PostedAt = DateTime.Now;

            _unitOfWork.Announcements.Add(announcement);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Announcement posted successfully." });
        }

        [HttpGet]
        public JsonResult GetAll(int classId)
        {
            var @class = _unitOfWork.Classes.Get(c => c.Id == classId);

            if (@class == null) return Json(new { success = false, message = "Class not found." });

            var announcements = _unitOfWork.Announcements
                .GetAll(a => a.ClassId == classId)
                .OrderByDescending(a => a.PostedAt)
                .Select(a => new
                {
                    a.Id,
                    a.ContentHtml,
                    PostedAt = a.PostedAt.ToString()
                })
                .ToList();

            return Json(announcements);
        }

        public JsonResult Delete(int id)
        {
            var announcement = _unitOfWork.Announcements.Get(a => a.Id == id);

            if (announcement == null) return Json(new { success = false, message = "Announcement not found." });

            _unitOfWork.Announcements.Remove(announcement);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Announcement removed successfully." });
        }
    }
}