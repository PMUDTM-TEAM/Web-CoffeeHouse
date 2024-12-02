namespace CoffeeHouse.Models
{
	public class Orders
	{
            public int Id { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; }
            public string PaymentStatus { get; set; }
            public string PaymentMethod { get; set; }
            public int Address_Id { get; set; }
            public int A_Id { get; set; }
            public DateTime CreatedAt { get; set; }
    }
}
