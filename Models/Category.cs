using System.ComponentModel.DataAnnotations;

namespace SmartPOS.Models
{
    public class Category
    {
        [Key] // এটি ডাটাবেজে Primary Key (ID) তৈরি করবে
        public int Id { get; set; }

        [Required(ErrorMessage = "ক্যাটাগরির নাম দিতেই হবে")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}