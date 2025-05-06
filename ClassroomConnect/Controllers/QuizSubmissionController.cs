using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class QuizSubmissionController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult ViewSubmission(int quizId, string studentId)
        {
            ViewData["Title"] = "Submitted Quiz Answers";

            var quiz = _unitOfWork.Quizzes.Get(q => q.Id == quizId, includeProperties: "Class");

            if (quiz == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _unitOfWork.Classes.Get(c => c.Id == quiz.ClassId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            quiz.Questions = _unitOfWork.QuizQuestions.GetAll(q => q.QuizId == quiz.Id).ToList();

            var quizSubmission = _unitOfWork.QuizSubmissions.Get(qs => qs.UserId.Equals(studentId) && qs.QuizId == quizId);

            if (quizSubmission == null) return NotFound();

            var quizAnswers = _unitOfWork.QuizAnswers.GetAll(qa => qa.QuizSubmissionId == quizSubmission.Id).ToList();

            var submittedQuiz = new SubmittedQuizVM
            {
                quiz = quiz,
                Answers = quizAnswers
            };

            return View(submittedQuiz);
        }

        public IActionResult Details(int id)
        {
            var quiz = _unitOfWork.Quizzes.Get(q => q.Id == id);

            if (quiz == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @class = _unitOfWork.Classes.Get(c => c.Id == quiz.ClassId);

            if (@class == null || @class.CreatedById != currentUserId) return RedirectToAction("Index", "Home");

            var classMembers = _unitOfWork.ClassMembers.GetAll(cm => cm.ClassId == quiz.ClassId, includeProperties: "User").ToList();

            var submissions = _unitOfWork.QuizSubmissions.GetAll(s => s.QuizId == id);

            ViewBag.Submissions = submissions;
            ViewBag.QuizId = id;

            return View(classMembers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submission(int id, QuizSubmissionVM model)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quiz = _unitOfWork.Quizzes.Get(q => q.Id == id, includeProperties: "Class");

            if (quiz == null) return NotFound();

            bool isQuizClosed = quiz?.CloseDate.HasValue == true && quiz.CloseDate < DateTime.Now; ;

            if (isQuizClosed)
            {
                ModelState.AddModelError("", "This quiz is now closed for submissions.");
                return RedirectToAction("Details", "Quiz", new { id });
            }

            if (ModelState.IsValid)
            {
                var quizSubmission = new QuizSubmission
                {
                    QuizId = id,
                    UserId = currentUserId,
                    SubmittedAt = DateTime.Now,
                };

                _unitOfWork.QuizSubmissions.Add(quizSubmission);
                _unitOfWork.Save();

                foreach (var answer in model.Answers)
                {
                    {
                        var quizAnswer = new QuizAnswer
                        {
                            QuestionId = answer.QuestionId,
                            AnswerText = answer.AnswerText,
                            QuizSubmissionId = quizSubmission.Id
                        };

                        _unitOfWork.QuizAnswers.Add(quizAnswer);
                    }
                }

                _unitOfWork.Save();

                TempData["success"] = "Quiz submitted successfully";

                return RedirectToAction("Details", "Quiz", new { id });
            }
            return RedirectToAction("Details", "Quiz", new { id });
        }
    }
}
