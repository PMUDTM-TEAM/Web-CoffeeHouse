namespace CoffeeHouse.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
		public float Price { get; set; }
		public int Order_Id { get; set; }
		public int Provar_Id { get; set; }
		public int Topping_Id { get; set; }
	}
}
