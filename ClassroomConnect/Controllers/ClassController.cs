using Classroom.DataAccess.Data;
using Classroom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    public class ClassController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        // GET: Classes
        public IActionResult Index()
        {
            return View(_db.Classes.ToList());
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

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.Description ??= string.Empty;

                // Set creation metadata
                @class.CreatedAt = DateTime.Now;
                @class.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets current user's ID

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

        private bool ClassExists(int id)
        {
            return _db.Classes.Any(e => e.Id == id);
        }
    }
}