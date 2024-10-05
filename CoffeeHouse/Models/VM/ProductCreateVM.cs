using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoffeeHouse.Models.VM
{
	public class ProductCreateVM
	{
		public Products products { get; set; }
		public IEnumerable<SelectListItem> categories { get; set; }
	}
}
