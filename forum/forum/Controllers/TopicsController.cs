using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using forum.Data;
using forum.Models;

namespace forum.Controllers
{
    public class TopicsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public TopicsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5; 
            
            var totalTopics = await _context.Topics.CountAsync();
            
            var topics = await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Replies)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalTopics / (double)pageSize);

            return View(topics);
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Название и содержимое темы обязательны.");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var topic = new Topic
            {
                Title = title,
                Content = content,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UserId = user.Id
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}