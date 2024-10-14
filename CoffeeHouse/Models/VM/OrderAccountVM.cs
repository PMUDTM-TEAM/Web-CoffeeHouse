namespace CoffeeHouse.Models.VM
{
	public class OrderAccountVM
	{
		public IEnumerable<Orders> Orders { get; set; }
		public IEnumerable<Accounts> Accounts { get; set; }
	}
}
