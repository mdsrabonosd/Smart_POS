using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using SmartPOS.Models;

namespace SmartPOS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

    
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // .Include(p => p.Category) ব্যবহারের কারণে আমরা প্রতিটা প্রোডাক্টের ক্যাটাগরির নামও একসাথে পেয়ে যাবো
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            return View(products);
        }
        // ১. প্রোডাক্ট তৈরির ফাঁকা ফর্ম এবং ক্যাটাগরি ড্রপডাউন দেখানোর মেথড (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // ডাটাবেজ থেকে সব ক্যাটাগরি নিয়ে আসা যেন ড্রপডাউনে দেখানো যায়
            var categories = await _context.Categories.ToListAsync();

            // ViewBag-এর মাধ্যমে ক্যাটাগরি লিস্টটি ভিউ পেজে পাঠিয়ে দেওয়া
            ViewBag.CategoryList = categories;

            return View();
        }

        // ২. ফর্মের ডাটা রিসিভ করে ডাটাবেজে নতুন প্রোডাক্ট সেভ করার মেথড (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product); // ডাটাবেজে প্রোডাক্ট যোগ করা
                await _context.SaveChangesAsync(); // ফাইনালি সেভ করা

                return RedirectToAction(nameof(Index)); // প্রোডাক্ট লিস্ট পেজে ফেরত পাঠানো
            }

            // যদি ফর্মের ডাটায় কোনো ভুল থাকে (Validation Fail), তবে ড্রপডাউনটি আবার লোড করতে হবে
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = categories;

            return View(product);
        }

        // ১. নির্দিষ্ট প্রোডাক্টের আগের ডাটা দিয়ে ফর্মটি পূরণ করে দেখানোর মেথড (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // ডাটাবেজ থেকে ওই ID-র প্রোডাক্টটি খুঁজে বের করা
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(); // প্রোডাক্ট না পাওয়া গেলে ৪0৪ এরর
            }

            // ড্রপডাউনের জন্য ক্যাটাগরি লিস্ট রেডি করা
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = categories;

            return View(product);
        }

        // ২. এডিট করা ডাটা রিসিভ করে ডাটাবেজে আপডেট করার মেথড (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Products.Update(product); // ডাটাবেজে ডাটা আপডেট মার্ক করা
                    await _context.SaveChangesAsync();  // পরিবর্তনটি সেভ করা
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // আপডেট শেষে লিস্ট পেজে ব্যাক করা
            }

            // ভ্যালিডেশন ফেইল করলে ড্রপডাউন পুনরায় লোড করা
            var categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryList = categories;

            return View(product);
        }
        // ১. ডিলিট করার আগে ডাটা নিশ্চিত করার জন্য কনফার্মেশন পেজ/ভিউ (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category) // ক্যাটাগরির নাম দেখানোর জন্য জয়েন করা
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // ২. ইউজার কনফার্ম করার পর ডাটাবেজ থেকে চিরতরে ডিলিট করার মেথড (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product); // ডাটাবেজ থেকে রিমুভ করা
                await _context.SaveChangesAsync(); // পরিবর্তন সেভ করা
            }

            return RedirectToAction(nameof(Index)); // লিস্ট পেজে ফেরত পাঠানো
        }
    }
}