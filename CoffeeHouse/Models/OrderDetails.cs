namespace CoffeeHouse.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public int Order_Id { get; set; }
		public int Provar_Id { get; set; }

        public List<int> Topping_Id { get; set; }

    }
}
