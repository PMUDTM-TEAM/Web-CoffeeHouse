namespace CoffeeHouse.Models
{
	public class Orders
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; }
		public Decimal TotalPrice { get; set; }
        public int Address_Id { get; set; }
        public int A_Id { get; set; }

	}
}
