using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class QuizController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            ViewData["Title"] = "Quiz Details";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = GetQuiz(id);

            if (quiz == null) return NotFound();

            if (currentUserId == quiz?.Class?.CreatedById)
                ViewBag.isCreator = true;

            ViewBag.HasSubmitted = IsQuizSubmitted(quiz, currentUserId);
            ViewBag.IsClosed = IsQuizClosed(quiz);

            if (quiz?.CloseDate.HasValue == true && !ViewBag.IsClosed)
            {
                var remainingTime = quiz.CloseDate.Value - DateTime.Now;
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

            return View(quiz);
        }

        public IActionResult Create(int classId)
        {
            ViewData["Title"] = "Create Quiz";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _unitOfWork.Classes.Get(c => c.Id == classId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            var quiz = new Quiz
            {
                Title = "",
                ClassId = classId
            };

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Instructions,DueDate,CloseDate,ClassId,Questions")] Quiz quiz)
        {
            if (quiz.DueDate == null)
                quiz.CloseDate = null;

            if (quiz.CloseDate != null && quiz.DueDate != null && quiz.CloseDate < quiz.DueDate)
            {
                ModelState.AddModelError("CloseDate", "Close Date must be equal to or later than Due Date.");
            }

            if (ModelState.IsValid)
            {
                if (quiz.Questions.Count == 0)
                {
                    ModelState.AddModelError("", "Please add at least one question before creating the quiz.");
                    return View(quiz);
                }

                quiz.PostedAt = DateTime.Now;

                _unitOfWork.Quizzes.Add(quiz);
                _unitOfWork.Save();

                TempData["success"] = "Quiz created successfully";

                return RedirectToAction("Details", "Class", new { id = quiz.ClassId });
            }

            return View(quiz);
        }

        public IActionResult Edit(int? id)
        {
            ViewData["Title"] = "Edit Quiz";

            if (id == null) return NotFound();
            var quiz = GetQuiz(id);
            if (quiz == null) return NotFound();

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,Instructions,DueDate,CloseDate,ClassId,Questions")] Quiz quiz)
        {
            if (id != quiz.Id) return NotFound();

            if (quiz.CloseDate != null && quiz.DueDate != null && quiz.CloseDate < quiz.DueDate)
            {
                ModelState.AddModelError("CloseDate", "Close Date must be equal to or later than Due Date.");
            }

            if (ModelState.IsValid)
            {
                if (quiz.DueDate == null)
                    quiz.CloseDate = null;

                if (quiz.Questions.Count == 0)
                {
                    ModelState.AddModelError("", "Please add at least one question before updating the quiz.");
                    return View(quiz);
                }

                try
                {
                    var quizFromDb = _unitOfWork.Quizzes.Get(q => q.Id == id);

                    if (quizFromDb == null) return NotFound();

                    _unitOfWork.Quizzes.Update(quizFromDb, quiz);
                    _unitOfWork.Save();

                    TempData["success"] = "Quiz updated successfully";

                    return RedirectToAction("Details", "Quiz", new { id = quizFromDb.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWork.Quizzes.Any(q => q.Id == id)) return NotFound();
                    else throw;
                }
            }

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var quiz = _unitOfWork.Quizzes.Get(q => q.Id == id);

            if (quiz == null)
            {
                return Json(new { success = false, message = "Quiz not found." });
            }

            var classId = quiz.ClassId;

            _unitOfWork.Quizzes.Remove(quiz);
            _unitOfWork.Save();

            return Json(new { redirectUrl = Url.Action("Details", "Class", new { id = classId }) });
        }

        [HttpGet]
        public JsonResult IsQuizClosed(int id)
        {
            var quiz = _unitOfWork.Quizzes.Get(q => q.Id == id);
            bool isClosed = IsQuizClosed(quiz);
            return Json(new { isClosed });
        }

        #region Helper methods

        private Quiz? GetQuiz(int? id)
        {
            Quiz? quiz = _unitOfWork.Quizzes.Get(q => q.Id == id, includeProperties: "Class");

            if (quiz != null)
                quiz.Questions = _unitOfWork.QuizQuestions.GetAll(q => q.QuizId == quiz.Id).ToList();

            return quiz;
        }

        private bool IsQuizSubmitted(Quiz? quiz, string? userId)
        {
            return _unitOfWork.QuizSubmissions.Any(q => q.QuizId == quiz.Id && q.UserId.Equals(userId));
        }

        private bool IsQuizClosed(Quiz? quiz)
        {
            return quiz?.CloseDate.HasValue == true 
                && quiz.CloseDate < DateTime.Now;
        }

        #endregion

    }
}
