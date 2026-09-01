using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;

namespace SmartPOS.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ১. কনস্ট্রাক্টরের মাধ্যমে ডাটাবেজ কানেকশন (Dependency Injection) নিয়ে আসা
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ২. ক্যাটাগরির লিস্ট দেখানোর মেথড (READ)
        public async Task<IActionResult> Index()
        {
            // ডাটাবেজ থেকে সব ক্যাটাগরি লিস্ট আকারে নিয়ে আসা
            var categories = await _context.Categories.ToListAsync();

            // লিস্টটি ভিউ (UI) এর কাছে পাঠিয়ে দেওয়া
            return View(categories);
        }
    }
}