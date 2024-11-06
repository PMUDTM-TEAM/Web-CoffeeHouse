using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class ProductService
    {
        private readonly ConnectDB _connectDB;

        public ProductService()
        {
            _connectDB = new ConnectDB(); 
        }

        // Lấy tất cả các sản phẩm
        public async Task<List<Products>> GetAllAsync()
        {
            // Truy vấn SQL để lấy tất cả các sản phẩm
            string query = @"
                        SELECT Id, Name, Slug, Image, Type, Cate_Id
                        FROM Product
                    ";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();
            var products = new List<Products>();

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
                                var product = new Products
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Slug = reader.GetString(reader.GetOrdinal("Slug")),
                                    Image = reader.GetString(reader.GetOrdinal("Image")),
                                    Type = reader.GetString(reader.GetOrdinal("Type")),
                                    Cate_Id = reader.GetInt32(reader.GetOrdinal("Cate_Id"))
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return products;
        }


        // Hàm lấy danh sách sản phẩm theo Cate_Id
        public async Task<List<Products>> GetProductByCateIdAsync(int cateId)
        {
            var query = @"SELECT Id, Name, Image, Type, Cate_Id, Slug FROM Product WHERE Cate_Id = @cateId";  

            var products = new List<Products>(); 
            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cateId", cateId);  

                        using (var reader = await command.ExecuteReaderAsync())  
                        {
                            while (await reader.ReadAsync())
                            {
                                var product = new Products
                                {
                                    Id = reader.GetInt32(0),  
                                    Name = reader.GetString(1),  
                                    Image = reader.GetString(2),  
                                    Type = reader.GetString(3), 
                                    Cate_Id = reader.GetInt32(4),  
                                    Slug = reader.GetString(5)  
                                };
                                products.Add(product); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  
                return new List<Products>();  
            }

            return products;  
        }

        // Lấy 4 sản phẩm type Hot
        public async Task<List<Products>> GetFourHotProduct()
        {
            var products = new List<Products>();
            var query = "SELECT TOP 4 Id, Name, Image, Type, Cate_Id, Slug FROM Product WHERE Type = 'Hot'";

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
                                var product = new Products
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Image = reader.GetString(2),
                                    Type = reader.GetString(3),
                                    Cate_Id = reader.GetInt32(4),
                                    Slug = reader.GetString(5)
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");
            }

            return products;
        }

        // Lấy 2 sản phẩm type New
        public async Task<List<Products>> GetTwoNewProduct()
        {
            var products = new List<Products>();
            var query = "SELECT TOP 2 Id, Name, Image, Type, Cate_Id, Slug FROM Product WHERE Type = 'New'";

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
                                var product = new Products
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Image = reader.GetString(2),
                                    Type = reader.GetString(3),
                                    Cate_Id = reader.GetInt32(4),
                                    Slug = reader.GetString(5)
                                };
                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");
            }

            return products;
        }
    }
}
