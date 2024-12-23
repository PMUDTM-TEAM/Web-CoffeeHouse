﻿namespace CoffeeHouse.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public int Order_Id { get; set; }
		public int Provar_Id { get; set; }

        public string ProductName { get; set; }
        public List<string> Toppings { get; set; }
        public string ProductImage { get; set; }

        public string ProductSize { get; set; }
        public decimal VariantPrice { get; set; }
        public decimal ToppingsPrice { get; set; }
        public decimal SizePrice { get; set; }
        public string SizeName { get; set; }
        public List<int> Topping_Id { get; set; }

    }
}
