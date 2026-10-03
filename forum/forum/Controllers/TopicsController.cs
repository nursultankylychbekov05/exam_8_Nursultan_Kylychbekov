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
        
        [HttpGet]
        public async Task<IActionResult> Details(int id, int page = 1)
        {
            int pageSize = 5; 

            var topic = await _context.Topics
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null) return NotFound();

            var totalReplies = await _context.Replies.Where(r => r.TopicId == id).CountAsync();

            var replies = await _context.Replies
                .Where(r => r.TopicId == id)
                .Include(r => r.User)
                .OrderBy(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var userReplyCounts = new Dictionary<string, int>();
            foreach (var reply in replies)
            {
                if (reply.UserId != null && !userReplyCounts.ContainsKey(reply.UserId))
                {
                    int count = await _context.Replies.CountAsync(r => r.UserId == reply.UserId);
                    userReplyCounts[reply.UserId] = count;
                }
            }

            ViewBag.Replies = replies;
            ViewBag.UserReplyCounts = userReplyCounts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalReplies / (double)pageSize);
            ViewBag.TopicId = id;

            return View(topic);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReply(int topicId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Сообщение не может быть пустым.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var reply = new Reply
            {
                TopicId = topicId,
                Content = content,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UserId = user.Id
            };

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();
            
            return Json(new
            {
                success = true,
                userName = user.UserName,
                userAvatar = user.Avatar,
                createdAt = reply.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                content = reply.Content
            });
        }
    }
}