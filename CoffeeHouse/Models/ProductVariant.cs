namespace CoffeeHouse.Models
{
	public class ProductVariant
	{
		public int Id { get; set; }
		public int Pro_Id { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public int Size_Id { get; set; }
	}
}
