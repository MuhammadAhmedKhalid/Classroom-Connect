using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class ClassController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        #region Endpoints

        public IActionResult Index()
        {
            ViewData["Title"] = "Created Classes";

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClasses = _unitOfWork.Classes.GetAll(c => c.CreatedById == currentUserId);

            return View(userClasses);
        }

        public IActionResult Details(int? id)
        {
            ViewData["Title"] = "Class Details";

            if (id == null) return NotFound();

            var @class = _unitOfWork.Classes.Get(m => m.Id == id);
            if (@class == null) return NotFound();

            //var classMembers = _db.ClassMembers
            //    .Where(cm => cm.ClassId == @class.Id)
            //    .Include(cm => cm.User)
            //    .ToList();

            //var assignments = _db.Assignments.Where(a => a.ClassId == @class.Id).ToList();
            //var quizzes = _db.Quizzes.Where(q => q.ClassId == @class.Id).ToList();

            var classDetails = new ClassDetailsVM
            {
                Class = @class,
                ClassMembers = null,
                Assignments = null,
                Quizzes = null
            };

            return View(classDetails);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Create Class";
            return View();
        }
        
        // The[Bind] attribute in ASP.NET Core MVC is used to control which properties of a model
        // are populated during model binding. Model binding is the process where ASP.NET Core takes
        // the data from an HTTP request (like form data, query string parameters, route data, etc.)
        // and automatically maps it to the parameters of your controller action methods.

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.Description ??= string.Empty;
                @class.CreatedAt = DateTime.Now;
                @class.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets current user's ID
                @class.ClassCode = GenerateUniqueClassCode();
                _unitOfWork.Classes.Add(@class);
                _unitOfWork.Save();

                TempData["success"] = "Class created successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public IActionResult Edit(int? id)
        {
            ViewData["Title"] = "Edit Class";

            if (id == null) return NotFound();
            var @class = _unitOfWork.Classes.Get(c => c.Id == id);
            if (@class == null) return NotFound();
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description")] Class @class)
        {
            if (id != @class.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var classFromDb = _unitOfWork.Classes.Get(c => c.Id == id);
                    if (classFromDb == null) return NotFound();

                    _unitOfWork.Classes.Update(classFromDb, @class);
                    _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public IActionResult Delete(int? id)
        {
            ViewData["Title"] = "Delete Class";

            if (id == null) return NotFound();

            var @class = _unitOfWork.Classes.Get(c => c.Id == id);

            if (@class == null) return NotFound();

            return View(@class);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var @class = _unitOfWork.Classes.Get(c => c.Id == id);

            if (@class != null)
            {
                _unitOfWork.Classes.Remove(@class);
                _unitOfWork.Save();
            }

            TempData["success"] = "Class deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Helper methods

        private bool ClassExists(int id)
        {
            return _unitOfWork.Classes.Any(c => c.Id == id);
        }

        private string GenerateUniqueClassCode()
        {
            using var rng = RandomNumberGenerator.Create();
            string code;
            bool isUnique;

            do
            {
                var codeLength = RandomNumberGenerator.GetInt32(5, 9);

                var randomBytes = new byte[codeLength];
                rng.GetBytes(randomBytes);

                code = new string(randomBytes.Select(b => AllowedChars[b % AllowedChars.Length]).ToArray());

                isUnique = !_unitOfWork.Classes.Any(c => c.ClassCode == code);
            }
            while (!isUnique);

            return code;
        }

        #endregion

    }
}