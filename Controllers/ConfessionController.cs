using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whisper.Models;
using Whisper.Services;

namespace Whisper.Controllers
{
    public class ConfessionController : Controller
    {
        // For demo purposes, we'll use a static list to store confessions in memory.
        // In a real application, you would use a database (e.g., Entity Framework Core with SQL Server, SQLite, etc.).
        private static List<Confession> _confessions = new List<Confession>();
        private readonly GeminiApiService _geminiApiService;

        public ConfessionController(GeminiApiService geminiApiService)
        {
            _geminiApiService = geminiApiService;
        }

        // GET: Confession/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Confession/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConfessionText")] Confession confession)
        {
            if (ModelState.IsValid)
            {
                // Get advice from the AI service
                confession.AdviceText = await _geminiApiService.GetAdvice(confession.ConfessionText);
                confession.Timestamp = System.DateTime.UtcNow; // Set timestamp
                confession.IsPublishedAnonymously = false; // Not published yet

                // Temporarily store the confession for the next view
                // In a real app, you might save to session or a temp database table
                TempData["CurrentConfessionId"] = confession.Id;
                _confessions.Add(confession); // Add to our in-memory list

                return RedirectToAction("Advice", new { id = confession.Id });
            }
            return View(confession);
        }

        // GET: Confession/Advice/{id}
        public IActionResult Advice(Guid id)
        {
            var confession = _confessions.FirstOrDefault(c => c.Id == id);
            if (confession == null)
            {
                return NotFound();
            }
            return View(confession);
        }

        // POST: Confession/Publish/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Publish(Guid id)
        {
            var confession = _confessions.FirstOrDefault(c => c.Id == id);
            if (confession == null)
            {
                return NotFound();
            }

            confession.IsPublishedAnonymously = true; // Mark as published
            return RedirectToAction("Index"); // Go to the list of published confessions
        }

        // GET: Confession/Index
        public IActionResult Index()
        {
            // Display only anonymously published confessions
            var publishedConfessions = _confessions.Where(c => c.IsPublishedAnonymously).ToList();
            return View(publishedConfessions);
        }
    }
}