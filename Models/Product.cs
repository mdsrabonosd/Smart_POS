using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "পণ্যের নাম দিতেই হবে")]
        public string Name { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "দাম ১ টাকার বেশি হতে হবে")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "স্টক ০ বা তার বেশি হতে হবে")]
        public int StockQuantity { get; set; }

        // Category টেবিলের সাথে সম্পর্ক (Relationship) তৈরি করা
        [Required(ErrorMessage = "একটি ক্যাটাগরি সিলেক্ট করুন")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}