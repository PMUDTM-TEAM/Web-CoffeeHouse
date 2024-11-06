using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class CategoryService
    {
        private readonly ConnectDB _connectDB;

        public CategoryService()
        {
            _connectDB = new ConnectDB(); 
        }

        // Hàm lấy Id của danh mục theo Slug
        public async Task<int> GetIdBySlugAsync(string slug)
        {
            var query = @"SELECT Id FROM Category WHERE Slug = @slug"; 
            var categoryId = 0;  

            var connectionString = _connectDB.GetConnectionString(); 

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@slug", slug); 

                       
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            categoryId = Convert.ToInt32(result);  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  
                return 0; 
            }

            return categoryId; 
        }

        // Hàm đếm số lượng sản phẩm theo từng danh mục
        public async Task<List<(string Name, string Slug, int Count)>> CountAllCategories()
        {
            var categoryCount = new List<(string, string, int)>();
            var query = @"SELECT c.Name AS Name, c.Slug AS Slug, COUNT(p.Id) AS PCount
                          FROM Category c
                          LEFT JOIN Product p ON c.Id = p.Cate_Id
                          GROUP BY c.Name, c.Slug"; 

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
                                var name = reader.GetString(0);    
                                var slug = reader.GetString(1);     
                                var count = reader.GetInt32(2);     

                                categoryCount.Add((name, slug, count));  
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); 
            }

            return categoryCount;  
        }

    }
}
