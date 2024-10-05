namespace CoffeeHouse.Models.VM
{
	public class ProductCategoryVM
	{
		public IEnumerable<Products> Products { get; set; }
		public IEnumerable<Category> Categories { get; set; }
	}
}
