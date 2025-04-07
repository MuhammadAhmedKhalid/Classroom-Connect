﻿using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ClassroomConnect.Controllers
{
    public class ClassController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        #region Endpoints

        // GET: Classes
        public IActionResult Index()
        {
            ViewData["Title"] = "Created Classes";

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userClasses = _db.Classes.Where(c => c.CreatedById == currentUserId).ToList();

            return View(userClasses);
        }

        // GET: Classes/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var @class = _db.Classes.FirstOrDefault(m => m.Id == id);
            if (@class == null) return NotFound();
            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
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
                _db.Add(@class);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        // GET: Classes/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var @class = _db.Classes.Find(id);
            if (@class == null) return NotFound();
            return View(@class);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description")] Class @class)
        {
            if (id != @class.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var classFromDb = _db.Classes.FirstOrDefault(c => c.Id == id);
                    if (classFromDb == null) return NotFound();
                    classFromDb.Name = @class.Name;
                    classFromDb.Description = @class.Description ?? string.Empty;

                    _db.SaveChanges();
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

        // GET: Classes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var @class = _db.Classes.FirstOrDefault(m => m.Id == id);

            if (@class == null) return NotFound();

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var @class = _db.Classes.Find(id);
            
            if (@class != null)
            {
                _db.Classes.Remove(@class);
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Helper methods

        private bool ClassExists(int id)
        {
            return _db.Classes.Any(e => e.Id == id);
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

                isUnique = !_db.Classes.Any(c => c.ClassCode == code);
            }
            while (!isUnique);

            return code;
        }

        #endregion

    }
}