namespace CoffeeHouse.Models.VM
{
	public class VariantSizeVM
	{
		public IEnumerable<Products> products { get; set; }
		public IEnumerable<Sizes> sizes { get; set; }
		public IEnumerable<ProductVariant> productVariants { get; set; }
	}
}
