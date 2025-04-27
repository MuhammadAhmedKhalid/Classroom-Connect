using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassroomConnect.Controllers
{
    [Authorize]
    public class JoinedClassController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            ViewData["Title"] = "Joined Classes";

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var joinedClasses = _unitOfWork.ClassMembers.GetAll(cm => cm.UserId == currentUserId, includeProperties: "Class");

            return View(joinedClasses);
        }

        public IActionResult Join()
        {
            ViewData["Title"] = "Join Class";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Join(JoinClassFormVM joinClassVM)
        {
            if (ModelState.IsValid)
            {
                var existingClass = _unitOfWork.Classes.Get(c => c.ClassCode == joinClassVM.ClassCode);

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

                var existingMembership = _unitOfWork.ClassMembers.Get(cm => cm.ClassId == existingClass.Id
                    && cm.UserId == userId);

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

                _unitOfWork.ClassMembers.Add(newMembership);
                _unitOfWork.Save();

                TempData["success"] = "Class joined successfully";

                return RedirectToAction("Index", "JoinedClass");
            }
            return View(joinClassVM);
        }

    }
}
