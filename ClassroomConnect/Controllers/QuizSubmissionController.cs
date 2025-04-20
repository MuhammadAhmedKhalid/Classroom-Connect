using Classroom.DataAccess.Data;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class QuizSubmissionController(ApplicationDbContext db) : Controller
    {

        private readonly ApplicationDbContext _db = db;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submission(int id, QuizSubmissionVM model)
        {
            // if is submission closed then dont allow 

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quiz = _db.Quizzes.Include(q => q.Class).FirstOrDefault(q => q.Id == id);

            if (quiz == null) return NotFound();


            if (ModelState.IsValid)
            {
                var quizSubmission = new QuizSubmission
                {
                    QuizId = id,
                    UserId = currentUserId,
                    SubmittedAt = DateTime.Now,
                };

                _db.QuizSubmissions.Add(quizSubmission);
                _db.SaveChanges();

                foreach (var answer in model.Answers)
                {
                    {
                        var quizAnswer = new QuizAnswer
                        {
                            QuestionId = answer.QuestionId,
                            AnswerText = answer.AnswerText,
                            QuizSubmissionId = quizSubmission.Id
                        };

                        _db.QuizAnswers.Add(quizAnswer);
                    }
                }
                
                _db.SaveChanges();

                TempData["success"] = "Quiz submitted successfully";

                return RedirectToAction("Details", "Quiz", new { id });
            }
            return RedirectToAction("Details", "Quiz", new { id });
        }
    }
}
