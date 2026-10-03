using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using forum.Data;
using forum.Models;

namespace forum.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var topicsCount = await _context.Topics.CountAsync(t => t.UserId == id);
            var repliesCount = await _context.Replies.CountAsync(r => r.UserId == id);

            ViewBag.TopicsCount = topicsCount;
            ViewBag.RepliesCount = repliesCount;

            return View(user);
        }
    }
}