using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // এই ইউজিং স্টেটমেন্টটি উপরে যোগ করবেন



namespace SmartPOS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalSales = await _context.Sales.SumAsync(s => s.GrandTotal);

            ViewBag.TotalOrders = await _context.Sales.CountAsync();

            ViewBag.TotalProducts = await _context.Products.CountAsync();

            var lowStockItems = await _context.Products
                .Where(p => p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();

            return View(lowStockItems);
        }
    }
}