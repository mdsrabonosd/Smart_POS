using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ডিপেন্ডেন্সি ইনজেকশন (Dependency Injection) এর মাধ্যমে ডাটাবেজ কন্টেক্সট নেওয়া
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // ১. লাইফটাইম মোট বিক্রির টাকা হিসাব করা (Sum of GrandTotal)
            ViewBag.TotalSales = await _context.Sales.SumAsync(s => s.GrandTotal);

            // ২. মোট কতটি অর্ডার বা ইনভয়েস জেনারেট হয়েছে (Count)
            ViewBag.TotalOrders = await _context.Sales.CountAsync();

            // ৩. ইনভেন্টরিতে মোট কতটি প্রোডাক্ট আছে
            ViewBag.TotalProducts = await _context.Products.CountAsync();

            // ৪. যে প্রোডাক্টগুলোর স্টক শেষ হয়ে আসছে (যেমন ৫ পিস বা তার কম), সেগুলোকে খুঁজে বের করা
            var lowStockItems = await _context.Products
                .Where(p => p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            return View(lowStockItems); // মডেল হিসেবে কম স্টকের লিস্টটি ভিউতে পাঠানো হলো
        }
    }
}