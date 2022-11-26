using System.ComponentModel.DataAnnotations;

namespace HandmadeStore.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter brand name"), StringLength(50), Display(Name = "Brand Name")]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
