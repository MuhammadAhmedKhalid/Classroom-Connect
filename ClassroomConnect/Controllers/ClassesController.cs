using Classroom.DataAccess.Data;
using Classroom.Models;
using Classroom.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class ClassesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClassesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassVM model)
    {
        if (model.Description == null)
        {
            model.Description = ""; // Convert null to empty string
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var newClass = new ClassroomData
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                TeacherId = user.Id,
                ClassCode = GenerateClassCode()
            };

            _context.Classes.Add(newClass);
            _context.SaveChanges();
            TempData["success"] = "Operation successful.";
            return RedirectToAction("Index", "Home");
        }

        TempData["error"] = "Opertion unsuccessful.";
        return RedirectToAction("Index", "Home");
    }

    private string GenerateClassCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}