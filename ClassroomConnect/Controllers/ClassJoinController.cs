using Classroom.DataAccess.Data;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    public class ClassJoinController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db = db;

        public IActionResult Index()
        {
            ViewData["Title"] = "Joined Classes";
            return View();
        }

        public IActionResult Join()
        {
            ViewData["Title"] = "Join Class";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinClassVM joinClassVM)
        {
            if (ModelState.IsValid)
            {
                var existingClass = _db.Classes.FirstOrDefault(c => c.ClassCode == joinClassVM.ClassCode);

                if (existingClass == null)
                {
                    ModelState.AddModelError("ClassCode", "Invalid Class Code.");
                    return View(joinClassVM);
                }
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (existingClass.CreatedById == userId)
                {
                    ModelState.AddModelError("ClassCode", "You cannot join a class you created.");
                    return View(joinClassVM);
                }

                var existingMembership = _db.ClassMembers.FirstOrDefault(cm => cm.ClassId == existingClass.Id && cm.UserId == userId);

                if (existingMembership != null)
                {
                    ModelState.AddModelError("ClassCode", "You are already a member of this class.");
                    return View(joinClassVM);
                }

                var newMembership = new ClassMember
                {
                    ClassId = existingClass.Id,
                    UserId = userId,
                    JoinedAt = DateTime.Now
                };

                _db.ClassMembers.Add(newMembership);
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(joinClassVM);
        }

    }
}
