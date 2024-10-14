namespace CoffeeHouse.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
		public float Price { get; set; }
		public int Order_Id { get; set; }
		public int Provar_Id { get; set; }
		public List<int> Topping_Id { get; set; }

        public string ProductName { get; set; }
        public List<string> Toppings { get; set; }
        public string ProductImage { get; set; }
    }
}
