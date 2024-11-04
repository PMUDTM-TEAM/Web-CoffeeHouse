using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class ProductVariantService
    {
        private readonly ConnectDB _connectDB;

        public ProductVariantService()
        {
            _connectDB = new ConnectDB();  // Sử dụng ConnectDB để lấy chuỗi kết nối
        }

        // Lấy tất cả ProductVariant với Size_Id = 1
        public async Task<List<ProductVariant>> GetProductVariantsBySizeId()
        {
            var productVariants = new List<ProductVariant>();
            var query = "SELECT Id, Pro_Id, Price, Quantity, Size_Id FROM ProductVariant WHERE Size_Id = @SizeId";

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SizeId", 1);  // Size_Id = 1

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var productVariant = new ProductVariant
                                {
                                    Id = reader.GetInt32(0),
                                    Pro_Id = reader.GetInt32(1),
                                    Price = reader.GetDecimal(2),
                                    Quantity = reader.GetInt32(3),
                                    Size_Id = reader.GetInt32(4)
                                };
                                productVariants.Add(productVariant);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");
            }

            return productVariants;
        }

    }
}
