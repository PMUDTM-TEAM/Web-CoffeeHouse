using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoffeeHouse.Models.VM
{
	public class VariantCreateVM
	{
		public IEnumerable<SelectListItem> sizes { get; set; }
		public ProductVariant productVariant { get; set; }
		public Products products { get; set; }

	}
}
