using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Models
{
    public class SaleDetail
    {
        public int Id { get; set; }

        // Foreign Key to Sale
        [Required]
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sale? Sale { get; set; }

        // Foreign Key to Product
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; } // Quantity * UnitPrice
    }
}