using System;
using System.ComponentModel.DataAnnotations;

namespace SmartPOS.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Invoice No")]
        public string InvoiceNumber { get; set; } = string.Empty; // e.g., INV-20260628-001

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = "Walking Customer"; // ডিফল্ট কাস্টমার

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; } // ৫% ভ্যাট বা ট্যাক্স

        [Required]
        [DataType(DataType.Currency)]
        public decimal GrandTotal { get; set; }

        // Navigation Property: একটি সেলের আন্ডারে অনেকগুলো আইটেম থাকতে পারে
        public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}