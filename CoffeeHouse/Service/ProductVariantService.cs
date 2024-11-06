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
            _connectDB = new ConnectDB();  
        }

        // Hàm lấy Id của ProductVariant dựa vào Pro_Id và Size_Id
        public async Task<int> GetProvarByProIdAndSizeId(int proId, int sizeId)
        {
            var query = @"SELECT Id FROM ProductVariant WHERE Pro_Id = @Pro_Id AND Size_Id = @Size_Id"; 

            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Pro_Id", proId);  
                        command.Parameters.AddWithValue("@Size_Id", sizeId); 

                        var result = await command.ExecuteScalarAsync();  

                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            return id;  
                        }
                        else
                        {
                            return -1;  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  // Xử lý lỗi
                return -1;  // Trả về -1 nếu có lỗi
            }
        }

        // Hàm lấy tất cả các biến thể sản phẩm
        public async Task<List<ProductVariant>> GetAllProductVariantsAsync()
        {
            var productVariants = new List<ProductVariant>();
            var query = @"SELECT Id, Pro_Id, Size_Id, Quantity, Price 
                          FROM ProductVariant"; 

            var connectionString = _connectDB.GetConnectionString(); 

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); 

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var productVariant = new ProductVariant
                                {
                                    Id = reader.GetInt32(0),      
                                    Pro_Id = reader.GetInt32(1),  
                                    Size_Id = reader.GetInt32(2),
                                    Quantity = reader.GetInt32(3), 
                                    Price = reader.GetDecimal(4)  
                                };

                                productVariants.Add(productVariant); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  
                return new List<ProductVariant>();  
            }

            return productVariants;  
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
