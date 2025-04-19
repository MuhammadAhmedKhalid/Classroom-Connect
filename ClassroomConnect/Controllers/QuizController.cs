using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class QuizController(ApplicationDbContext db) : Controller
    {

        private readonly ApplicationDbContext _db = db;

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            ViewData["Title"] = "Quiz Details";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = GetQuiz(id);

            if (currentUserId == quiz?.Class?.CreatedById)
                ViewBag.isCreator = true;

            ViewBag.HasSubmitted = IsQuizSubmitted(quiz);
            ViewBag.IsClosed = IsQuizClosed(quiz);

            return View(quiz);
        }

        public IActionResult Create(int classId)
        {
            ViewData["Title"] = "Create Quiz";

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

                _db.Quizzes.Add(quiz);
                _db.SaveChanges();

                TempData["success"] = "Quiz created successfully";

                return RedirectToAction("Details", "Class", new { id = quiz.ClassId });
            }
            return View(quiz);
        }

        #region Helper methods

        private Quiz? GetQuiz(int? id)
        {
            Quiz quiz = _db.Quizzes.Include(q => q.Class).FirstOrDefault(q => q.Id == id);

            if (quiz != null)
                quiz.Questions = _db.QuizQuestions.Where(q => q.QuizId == quiz.Id).ToList();

            return quiz;
        }

        private bool IsQuizSubmitted(Quiz? quiz)
        {
            return false;
        }

        private bool IsQuizClosed(Quiz? quiz)
        {
            return quiz?.CloseDate.HasValue == true 
                && quiz.CloseDate < DateTime.Now;
        }

        #endregion

    }
}
