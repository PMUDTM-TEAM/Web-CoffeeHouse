
namespace CoffeeHouse.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public int Cate_Id { get; set; } 

        public List<ProductVariant> ProductVariants { get; set; }
        public List<Sizes> Sizes { get; set; }
        public List<Category> Categories { get; set; }
    }
}

