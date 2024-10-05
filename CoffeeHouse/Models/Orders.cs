namespace CoffeeHouse.Models
{
	public class Orders
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Status { get; set; }
		public float TotalPrice { get; set; }
		public int A_Id { get; set; }

	}
}
