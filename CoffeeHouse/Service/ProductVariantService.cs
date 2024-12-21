using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data;
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

        // Hàm kiểm tra số lượng của sản phẩm
        public async Task<(int, int, string, string)> checkQuantityProvar(int Id, int quantity)
        {
            // Truy vấn để lấy thông tin từ các bảng liên quan
            var query = @"
                 SELECT 
                     pv.Quantity, 
                     p.Name AS ProductName, 
                     s.Size AS SizeName
                 FROM ProductVariant pv
                 INNER JOIN Product p ON pv.Pro_Id = p.Id
                 INNER JOIN Size s ON pv.Size_Id = s.Id
                 WHERE pv.Id = @Id";

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Lấy thông tin từ kết quả truy vấn
                                int currentQuantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                                string nameProduct = reader.GetString(reader.GetOrdinal("ProductName"));
                                string sizeName = reader.GetString(reader.GetOrdinal("SizeName"));

                                if (currentQuantity >= quantity)
                                {
                                    // Trả về 1 nếu đủ số lượng
                                    return (1, currentQuantity, nameProduct, sizeName);
                                }
                                else
                                {
                                    // Trả về 0 nếu không đủ số lượng, đồng thời trả về số lượng còn lại
                                    return (0, currentQuantity, nameProduct, sizeName);
                                }
                            }
                            else
                            {
                                // Không tìm thấy Id trong bảng ProductVariant
                                return (0, 0, string.Empty, string.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return (0, 0, string.Empty, string.Empty);
            }
        }

        // Hàm trừ quantity sản phẩm khi mua
        public async Task removeQuantityWhenBy(int Id, int quantity)
        {
            var queryGetQuantity = "SELECT Quantity FROM ProductVariant WHERE Id = @Id";
            var queryUpdateQuantity = "UPDATE ProductVariant SET Quantity = @NewQuantity WHERE Id = @Id";

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Lấy quantity hiện tại
                    int currentQuantity;
                    using (var commandGet = new SqlCommand(queryGetQuantity, connection))
                    {
                        commandGet.Parameters.AddWithValue("@Id", Id);

                        var result = await commandGet.ExecuteScalarAsync();

                        if (result == null || !int.TryParse(result.ToString(), out currentQuantity))
                        {
                            Console.WriteLine("Id không tồn tại hoặc không lấy được quantity.");
                            return;
                        }
                    }

                    // Kiểm tra nếu quantity không đủ để trừ
                    if (currentQuantity < quantity)
                    {
                        Console.WriteLine("Quantity không đủ để trừ.");
                        return;
                    }

                    // Cập nhật quantity mới
                    int newQuantity = currentQuantity - quantity;

                    using (var commandUpdate = new SqlCommand(queryUpdateQuantity, connection))
                    {
                        commandUpdate.Parameters.AddWithValue("@NewQuantity", newQuantity);
                        commandUpdate.Parameters.AddWithValue("@Id", Id);

                        await commandUpdate.ExecuteNonQueryAsync();
                    }

                    Console.WriteLine("Cập nhật quantity thành công.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }

        public async Task<List<string>> GetSizesByProductNameAsync(string productName)
        {
            var sizes = new List<string>();
            var query = @"
    SELECT DISTINCT s.Size
    FROM ProductVariant pv
    INNER JOIN Size s ON pv.Size_Id = s.Id
    INNER JOIN Product p ON pv.Pro_Id = p.Id
    WHERE p.Name LIKE @ProductName";  // Sử dụng LIKE và tham số chuẩn

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = "%" + productName + "%";  // Đảm bảo sử dụng NVarChar cho Unicode

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var sizeName = reader.GetString(0);
                                sizes.Add(sizeName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu kích cỡ: {ex.Message}");
            }

            return sizes;
        }


    }
}
