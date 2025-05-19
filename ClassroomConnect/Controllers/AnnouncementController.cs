using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ClassroomConnect.Controllers
{
    public class AnnouncementController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Post([Bind("ContentHtml,ClassId")] Announcement announcement)
        {
            if (!IsValidHtmlContent(announcement.ContentHtml)) return Json(new { success = false, message = "Announcement content cannot be empty." });

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update([Bind("Id,ContentHtml")] Announcement announcement)
        {
            if (!IsValidHtmlContent(announcement.ContentHtml)) return Json(new { success = false, message = "Announcement content cannot be empty." });

            var announcementFromDb = _unitOfWork.Announcements.Get(a => a.Id == announcement.Id);
            if (announcementFromDb == null) return Json(new { success = false, message = "Announcement not found." });

            _unitOfWork.Announcements.Update(announcementFromDb, announcement);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Announcement updated successfully." });
        }

        public JsonResult Delete(int id)
        {
            var announcement = _unitOfWork.Announcements.Get(a => a.Id == id);

            if (announcement == null) return Json(new { success = false, message = "Announcement not found." });

            _unitOfWork.Announcements.Remove(announcement);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Announcement removed successfully." });
        }

        #region Helper methods

        private bool IsValidHtmlContent(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return false;

            var plainText = Regex.Replace(html, "<.*?>", string.Empty); // non-greedy <.*?> // greedy <.*>

            return !string.IsNullOrWhiteSpace(plainText.Trim());
        }

        #endregion
    }
}