using Classroom.DataAccess.Repository.IRepository;
using Classroom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomConnect.Controllers
{
    public class ClassMemberController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Add(int classId, string identifier)
        {
            var @class = _unitOfWork.Classes.Get(c => c.Id == classId);
            if (@class == null) return Json(new { success = false, message = "Class not found." });

            var user = _userManager.FindByEmailAsync(identifier).Result;
            if (user == null) return Json(new { success = false, message = $"No user found with the email: {identifier}." });

            bool isAlreadyMember = _unitOfWork.ClassMembers.Any(cm => cm.ClassId == classId && cm.UserId == user.Id);
            if (isAlreadyMember) return Json(new { success = false, message = $"{user.Name} is already a member of this class." });

            bool isCreator = @class.CreatedById.Equals(user.Id);
            if (isCreator) return Json(new { success = false, message = "The class creator cannot add themselves as a member." });

            var classMember = new ClassMember
            {
                ClassId = classId,
                UserId = user.Id,
                JoinedAt = DateTime.Now,
            };

            _unitOfWork.ClassMembers.Add(classMember);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"{user.Email ?? user.Id} has been added to the class." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int classId, string memberId)
        {
            var @class = _unitOfWork.Classes.Get(c => c.Id == classId);

            if (@class == null) return Json(new { success = false, message = "Class not found." });

            var classMember = _unitOfWork.ClassMembers.Get(cm => cm.ClassId == classId && cm.UserId == memberId, includeProperties: "User");

            if (classMember == null) return Json(new { success = false, message = "Member not found." });

            _unitOfWork.ClassMembers.Remove(classMember);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Member removed successfully." });
        }
    }
}
