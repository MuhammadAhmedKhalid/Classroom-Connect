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

        public IActionResult ViewSubmission(int quizId, string studentId)
        {
            ViewData["Title"] = "Submitted Quiz Answers";

            var quiz = _db.Quizzes.Include(q => q.Class).FirstOrDefault(q => q.Id == quizId);

            if (quiz == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _db.Classes.FirstOrDefault(c => c.Id == quiz.ClassId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            quiz.Questions = _db.QuizQuestions.Where(q => q.QuizId == quiz.Id).ToList();

            var quizSubmission = _db.QuizSubmissions.FirstOrDefault(qs => qs.UserId.Equals(studentId) && qs.QuizId == quizId);

            if (quizSubmission == null) return NotFound();

            var quizAnswers = _db.QuizAnswers.Where(qa => qa.QuizSubmissionId == quizSubmission.Id).ToList();

            var submittedQuiz = new SubmittedQuizVM
            {
                quiz = quiz,
                Answers = quizAnswers
            };

            return View(submittedQuiz);
        }

        public IActionResult Details(int id)
        {
            var quiz = _db.Quizzes.FirstOrDefault(q => q.Id == id);

            if (quiz == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _db.Classes.FirstOrDefault(c => c.Id == quiz.ClassId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            var classMembers = _db.ClassMembers
                .Where(cm => cm.ClassId == quiz.ClassId)
                .Include(cm => cm.User)
                .ToList();

            var submissions = _db.QuizSubmissions
                .Where(s => s.QuizId == id)
                .ToList();

            ViewBag.Submissions = submissions;
            ViewBag.QuizId = id;

            return View(classMembers);
        }

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
