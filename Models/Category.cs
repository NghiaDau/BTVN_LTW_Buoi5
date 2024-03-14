using System.ComponentModel.DataAnnotations;

namespace BTVN5.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required,StringLength(50)]
        public string CategoryName { get; set; }
        public List<Product>? Products { get; set; }
    }
}
