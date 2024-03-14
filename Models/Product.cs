using System.ComponentModel.DataAnnotations;

namespace BTVN5.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required,StringLength(150)]
        public string Title { get; set; }
        [StringLength(150)]
        public string Author { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; }
        [StringLength(150)]
        public string? Image { get; set; }
       
        public int CategpryId { get; set; }
        public Category? Categpry { get; set; }
    }
}
