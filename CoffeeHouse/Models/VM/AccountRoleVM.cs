namespace CoffeeHouse.Models.VM
{
	public class AccountRoleVM
	{
		public IEnumerable<Role> roles { get; set; }
		public IEnumerable<Accounts> accounts { get; set; }
	}
}
