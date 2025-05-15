using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (assignment.DueDate == null)
                assignment.CloseDate = null;

            if (assignment.CloseDate != null && assignment.DueDate != null && assignment.CloseDate < assignment.DueDate)
            {
                ModelState.AddModelError("CloseDate", "Close Date must be equal to or later than Due Date.");
            }

            if (ModelState.IsValid)
            {
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

            if (assignment?.CloseDate.HasValue == true && !ViewBag.IsClosed)
            {
                var remainingTime = assignment.CloseDate.Value - DateTime.Now;
                if (remainingTime.TotalSeconds > 0)
                {
                    ViewBag.RemainingSeconds = (int)Math.Ceiling(remainingTime.TotalSeconds);
                }
                else
                {
                    ViewBag.RemainingSeconds = 0; 
                    ViewBag.IsClosed = true; 
                }
            }
            else
            {
                ViewBag.RemainingSeconds = 0;
            }

            return View(assignment);
        }

        public IActionResult Edit(int? id)
        {
            ViewData["Title"] = "Edit Assignment";

            if (id == null) return NotFound();
            var assignment = _unitOfWork.Assignments.Get(a => a.Id == id);
            if (assignment == null) return NotFound();
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (assignment.CloseDate != null && assignment.DueDate != null && assignment.CloseDate < assignment.DueDate)
            {
                ModelState.AddModelError("CloseDate", "Close Date must be equal to or later than Due Date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var assignmentFromDb = _unitOfWork.Assignments.Get(a => a.Id == id);

                    if (assignmentFromDb == null) return NotFound();

                    _unitOfWork.Assignments.Update(assignmentFromDb, assignment);
                    _unitOfWork.Save();

                    TempData["success"] = "Assignment updated successfully";

                    return RedirectToAction("Details", "Class", new { id = assignmentFromDb.ClassId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWork.Assignments.Any(a => a.Id == id)) return NotFound();
                    else throw;
                }
            }

            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var assignment = _unitOfWork.Assignments.Get(a => a.Id == id);

            if (assignment == null)
            {
                return Json(new { success = false, message = "Assignment not found." });
            }
            
            var classId = assignment.ClassId;

            _unitOfWork.Assignments.Remove(assignment);
            _unitOfWork.Save();

            return Json(new { redirectUrl = Url.Action("Details", "Class", new { id = classId }) });
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
