using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class QuizController(ApplicationDbContext db) : Controller
    {

        private readonly ApplicationDbContext _db = db;

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

    }
}
