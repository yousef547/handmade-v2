using System.ComponentModel.DataAnnotations;

namespace HandmadeStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter category name"), StringLength(50), Display(Name = "Category Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter category name"), StringLength(50), Display(Name = "Category Arabic Name")]
        public string ArabicName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
