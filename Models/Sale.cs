using System;
using System.ComponentModel.DataAnnotations;

namespace SmartPOS.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Invoice No")]
        public string InvoiceNumber { get; set; } = string.Empty; 

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = "Walking Customer"; 

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; } 

        [Required]
        [DataType(DataType.Currency)]
        public decimal GrandTotal { get; set; }

        public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}