using Classroom.DataAccess.Data;
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

            if (currentUserId == quiz?.Class?.CreatedById)
                ViewBag.isCreator = true;

            ViewBag.HasSubmitted = IsQuizSubmitted(quiz, currentUserId);
            ViewBag.IsClosed = IsQuizClosed(quiz);

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
        public IActionResult Create([Bind("Title,Instructions,DueDate,CloseDate,ClassId, Questions")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                if (quiz.DueDate == null)
                    quiz.CloseDate = null;

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

        #region Helper methods

        private Quiz? GetQuiz(int? id)
        {
            Quiz? quiz = _unitOfWork.Quizzes.Get(q => q.Id == id, includeProperties: "Class");

            if (quiz != null)
                quiz.Questions = _unitOfWork.QuizQuestions.GetAll(q => q.QuizId == quiz.Id).ToList();

            return quiz;
        }

        private bool IsQuizSubmitted(Quiz? quiz, string userId)
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
