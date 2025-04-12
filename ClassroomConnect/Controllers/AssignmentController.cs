using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    public class AssignmentController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : Controller
    {

        private readonly ApplicationDbContext _db = db;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public IActionResult Create(int classId)
        {
            ViewData["Title"] = "Create Assignment";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _db.Classes.FirstOrDefault(c => c.Id == classId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            var assignment = new Assignment
            {
                Title = "",
                ClassId = classId
            };

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Instructions,DueDate,ClassId")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.Instructions ??= string.Empty;
                assignment.PostedAt = DateTime.Now;

                _db.Add(assignment);
                _db.SaveChanges();

                TempData["success"] = "Assignment posted successfully";

                return RedirectToAction("Details", "Class", new { id = assignment.ClassId });
            }
            return View(assignment);
        }

        public IActionResult Details(int? id)
        {
            ViewData["Title"] = "Assignment Details";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null) return NotFound();

            var assignment = GetAssignment(id);

            ViewBag.HasSubmitted = _db.AssignmentSubmissions.Any(s => s.AssignmentId == id && s.UserId == currentUserId);

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submission(int id, IFormFile wordDocument)
        {
            if (wordDocument == null || wordDocument.Length == 0)
            {
                ModelState.AddModelError("WordDocument", "Please select a file.");

                var assignment = GetAssignment(id);
                
                return View("Details", assignment); 
            }

            if (wordDocument.Length > 16 * 1024 * 1024) // 16MB
            {
                ModelState.AddModelError("WordDocument", "File size exceeds the maximum limit of 16MB.");

                var assignment = GetAssignment(id);

                return View("Details", assignment);
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

                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("WordDocument", "Failed to upload the file. Error: " + ex.Message);

                var assignment = GetAssignment(id);

                return View("Details", assignment);
            }
        }

        #region Helper methods

        private Assignment GetAssignment(int? id)
        {
            return _db.Assignments
                .Include(a => a.Class)
                .FirstOrDefault(m => m.Id == id);
        }

        #endregion

    }
}
