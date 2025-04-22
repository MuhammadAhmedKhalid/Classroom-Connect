using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class AssignmentSubmissionController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : Controller
    {

        private readonly ApplicationDbContext _db = db;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public IActionResult Details(int assignmentId)
        {
            var assignment = _db.Assignments.FirstOrDefault(a => a.Id == assignmentId);

            if (assignment == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _db.Classes.FirstOrDefault(c => c.Id == assignment.ClassId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            var classMembers = _db.ClassMembers
                .Where(cm => cm.ClassId == assignment.ClassId)
                .Include(cm => cm.User)
                .ToList();

            var submissions = _db.AssignmentSubmissions
                .Where(s => s.AssignmentId == assignmentId)
                .ToList();

            ViewBag.Submissions = submissions;
            ViewBag.AssignmentId = assignmentId; 

            return View(classMembers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submission(int id, IFormFile wordDocument)
        {
            var assignment = _db.Assignments.Find(id);
            bool isSubmissionClosed = assignment?.CloseDate.HasValue == true && assignment.CloseDate < DateTime.Now; ;
            
            if (isSubmissionClosed)
            {
                ModelState.AddModelError("WordDocument", "This assignment is now closed for submissions.");

                return RedirectToAction("Details", "Assignment", new { id });
            }

            if (wordDocument == null || wordDocument.Length == 0)
            {
                ModelState.AddModelError("WordDocument", "Please select a file.");

                return RedirectToAction("Details", "Assignment", new { id });
            }

            if (wordDocument.Length > 16 * 1024 * 1024) // 16MB
            {
                ModelState.AddModelError("WordDocument", "File size exceeds the maximum limit of 16MB.");

                return RedirectToAction("Details", "Assignment", new { id });
            }

            string fileName = wordDocument.FileName;
            string uniqueFileName = $"{Guid.NewGuid().ToString()}_{fileName}";
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "submissions"); // wwwroot/submissions
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    wordDocument.CopyTo(fileStream);
                }

                var submission = new AssignmentSubmission
                {
                    AssignmentId = id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    SubmittedAt = DateTime.Now,
                    FilePath = uniqueFileName,
                    FileName = fileName,
                };

                _db.AssignmentSubmissions.Add(submission);
                _db.SaveChanges();

                TempData["success"] = "Assignment submitted successfully";

                return RedirectToAction("Details", "Assignment", new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("WordDocument", "Failed to upload the file. Error: " + ex.Message);

                return RedirectToAction("Details", "Assignment", new { id });
            }
        }

        public IActionResult DownloadSubmission(int submissionId)
        {
            var submission = _db.AssignmentSubmissions.FirstOrDefault(s => s.Id == submissionId);

            if (submission == null || string.IsNullOrEmpty(submission.FilePath)) return NotFound();

            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "submissions");
            var filePath = Path.Combine(uploadFolder, submission.FilePath); 
            var fileName = submission.FileName;

            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, GetContentType(filePath), fileName);
        }

        #region Helper methods
        
        private string GetContentType(string path)
        {
            // This class, provided by ASP.NET Core, is designed to map file extensions (like ".jpg", ".pdf", ".txt") to their corresponding MIME content types.
            var provider = new FileExtensionContentTypeProvider();

            //  This method attempts to get the content type for the file specified by the path.
            if (!provider.TryGetContentType(path, out var contentType))
            {
                // This sets the contentType variable to "application/octet-stream".
                // This is a generic MIME type used for arbitrary binary data when the specific type cannot be identified.
                // It acts as a safe default.
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        
        #endregion
    }
}
