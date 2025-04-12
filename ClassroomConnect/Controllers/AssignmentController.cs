using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    public class AssignmentController(ApplicationDbContext db) : Controller
    {

        private readonly ApplicationDbContext _db = db;

        public IActionResult Create(int classId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var @class = _db.Classes.FirstOrDefault(c => c.Id == classId);

            if (@class == null || @class.CreatedById != currentUserId)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Create Assignment";

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

                _db.Add(assignment);
                _db.SaveChanges();

                TempData["success"] = "Assignment posted successfully";

                return RedirectToAction("Details", "Class", new { id = assignment.ClassId });
            }
            return View(assignment);
        }

    }
}
