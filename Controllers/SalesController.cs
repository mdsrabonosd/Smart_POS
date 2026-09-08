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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .ToListAsync();

            ViewBag.Products = products;

            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int todaySalesCount = await _context.Sales
                .CountAsync(s => s.SaleDate.Date == DateTime.Today) + 1;

            ViewBag.InvoiceNumber = $"INV-{datePart}-{todaySalesCount:D4}";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sale saleData)
        {
            if (saleData == null || !saleData.SaleDetails.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Cart is empty or invalid data."
                });
            }

            try
            {
                saleData.SaleDate = DateTime.Now;

                foreach (var detail in saleData.SaleDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);

                    if (product == null || product.StockQuantity < detail.Quantity)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = $"Product {product?.Name ?? "Unknown"} out of stock!"
                        });
                    }

                    product.StockQuantity -= detail.Quantity;

                    detail.UnitPrice = product.Price;
                    detail.TotalPrice = detail.Quantity * product.Price;
                }

                _context.Sales.Add(saleData);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Sale completed successfully!",
                    invoiceId = saleData.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
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