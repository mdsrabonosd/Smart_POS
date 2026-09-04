using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using SmartPOS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPOS.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ১. বিক্রির মেইন ইন্টারফেস (POS Counter)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // কাউন্টারে দেখানোর জন্য সব প্রোডাক্ট লোড করা হচ্ছে
            var products = await _context.Products.Where(p => p.StockQuantity > 0).ToListAsync();
            ViewBag.Products = products;

            // অটোমেটিক একটি ইউনিক ইনভয়েস নাম্বার জেনারেট করা (যেমন: INV-20260628-XXXX)
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int todaySalesCount = await _context.Sales.CountAsync(s => s.SaleDate.Date == DateTime.Today) + 1;
            ViewBag.InvoiceNumber = $"INV-{datePart}-{todaySalesCount:D4}";

            return View();
        }

        // ২. জাভাস্ক্রিপ্ট থেকে পাঠানো অর্ডারের ডাটা রিসিভ এবং সেভ করা (POST via AJAX)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sale saleData)
        {
            if (saleData == null || !saleData.SaleDetails.Any())
            {
                return BadRequest(new { success = false, message = "Cart is empty or invalid data." });
            }

            try
            {
                // ইনভয়েস সেট এবং তারিখ ফিক্স করা
                saleData.SaleDate = DateTime.Now;

                // লুপ চালিয়ে স্টক আপডেট এবং ক্যালকুলেশন ভেরিফাই করা
                foreach (var detail in saleData.SaleDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null || product.StockQuantity < detail.Quantity)
                    {
                        return BadRequest(new { success = false, message = $"Product {product?.Name ?? "Unknown"} out of stock!" });
                    }

                    // স্টক থেকে কোয়ান্টিটি মাইনাস করা
                    product.StockQuantity -= detail.Quantity;

                    // রিলেশন ট্র্যাকিং নিশ্চিত করা
                    detail.UnitPrice = product.Price;
                    detail.TotalPrice = detail.Quantity * product.Price;
                }

                // ডাটাবেজে মাস্টার এবং ডিটেইলস একসাথে সেভ
                _context.Sales.Add(saleData);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sale completed successfully!", invoiceId = saleData.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        // ৩. বিক্রির পর নির্দিষ্ট ইনভয়েসের ডিটেইলস এবং প্রিন্ট ভিউ দেখানো (GET)
        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            // ডাটাবেজ থেকে মূল Sale এবং তার সাথে যুক্ত SaleDetails ও Product-এর নাম একবারে নিয়ে আসা (Eager Loading)
            var sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
    }
}