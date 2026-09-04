using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using SmartPOS.Models;

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

            return View(categories);
        }
        // ১. এই মেথডটি শুধু নতুন ক্যাটাগরি তৈরির ফর্মটি স্ক্রিনে দেখাবে (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ২. এই মেথডটি ফর্মে সাবমিট করা ডাটা রিসিভ করে ডাটাবেজে সেভ করবে (POST)
        [HttpPost]
        [ValidateAntiForgeryToken] // সিকিউরিটি অ্যাটাক (CSRF) ঠেকানোর জন্য এটি ইন্টারভিউতে খুব গুরুত্বপূর্ণ
        public async Task<IActionResult> Create(Category category)
        {
            // আমরা মডেল ক্লাসে যে ভ্যালিডেশন দিয়েছিলাম (যেমন: Name Required), তা এখানে চেক হচ্ছে
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category); // ডাটাবেজ ট্র্যাকিং-এ যোগ করা
                await _context.SaveChangesAsync(); // ডাটাবেজে ফাইনালি সেভ করা

                return RedirectToAction(nameof(Index)); // সেভ শেষে লিস্ট পেজে ফেরত পাঠানো
            }

            // ডাটা যদি ভ্যালিড না হয়, তবে এরর মেসেজ সহ ফর্মটি আবার দেখাবে
            return View(category);
        }
        // ১. এডিটের ফাঁকা ফর্ম বা আগের ডাটা সহ ফর্ম দেখানোর মেথড (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // যদি ইউআরএল (URL)-এ কোনো আইডি না থাকে, তবে এরর দেখাবে
            if (id == null)
            {
                return NotFound();
            }

            // ডাটাবেজ থেকে ওই আইডি-র ক্যাটাগরিটি খুঁজে বের করা
            var category = await _context.Categories.FindAsync(id);

            // ক্যাটাগরি যদি ডাটাবেজে না পাওয়া যায়
            if (category == null)
            {
                return NotFound();
            }

            // পাওয়া গেলে ডাটা সহ ভিউ পেজে পাঠানো
            return View(category);
        }

        // ২. ফর্ম সাবমিট হওয়ার পর ডাটাবেজে আপডেট করার মেথড (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            // ইউআরএল এর আইডি আর ফর্মের ভেতরের আইডি এক কি না তা নিশ্চিত করা
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Update(category); // ডাটাবেজে আপডেট স্টেট সেট করা
                    await _context.SaveChangesAsync();    // ডাটাবেজে সেভ করা
                }
                catch (DbUpdateConcurrencyException)
                {
                    // যদি একই সময়ে অন্য কেউ এটা ডিলিট বা চেঞ্জ করে ফেলে তার জন্য সিকিউরিটি চেক
                    if (!_context.Categories.Any(e => e.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // আপডেট শেষে লিস্টে ফেরত পাঠানো
            }

            return View(category);
        }
        // ক্যাটাগরি ডিলিট করার মেথড (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ডাটাবেজ থেকে নির্দিষ্ট ক্যাটাগরি খুঁজে বের করা
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category); // ডাটাবেজ থেকে রিমুভ ট্র্যাকিং করা
                await _context.SaveChangesAsync();    // ডাটাবেজ থেকে ফাইনালি ডিলিট করা
            }

            return RedirectToAction(nameof(Index)); // লিস্ট পেজে ফেরত পাঠানো
        }
    }
}