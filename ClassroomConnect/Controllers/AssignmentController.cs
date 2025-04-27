using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class AssignmentController(IUnitOfWork unitOfWork) : Controller
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Create(int classId)
        {
            ViewData["Title"] = "Create Assignment";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _unitOfWork.Classes.Get(c => c.Id == classId);

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
        public IActionResult Create([Bind("Title,Instructions,DueDate,CloseDate,ClassId")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {

                if (assignment.DueDate == null)
                    assignment.CloseDate = null;

                assignment.Instructions ??= string.Empty;
                assignment.PostedAt = DateTime.Now;

                _unitOfWork.Assignments.Add(assignment);
                _unitOfWork.Save();

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

            ViewBag.HasSubmitted = _unitOfWork.AssignmentSubmissions.Any(s => s.AssignmentId == id && s.UserId == currentUserId);
            ViewBag.IsClosed = IsAssignmentClosed(assignment);

            return View(assignment);
        }

        [HttpGet]
        public JsonResult IsAssignmentClosed(int id)
        {
            var assignment = _unitOfWork.Assignments.Get(a => a.Id == id);
            bool isClosed = IsAssignmentClosed(assignment);
            return Json(new { isClosed });
        }

        #region Helper methods

        private Assignment? GetAssignment(int? id)
        {
            return _unitOfWork.Assignments.Get(m => m.Id == id, includeProperties: "Class");
        }

        private bool IsAssignmentClosed(Assignment? assignment)
        {
            return assignment?.CloseDate.HasValue == true && assignment.CloseDate < DateTime.Now;
        }

        #endregion

    }
}
