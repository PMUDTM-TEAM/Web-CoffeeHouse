using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class SizeService
    {
        private readonly ConnectDB _connectDB;

        public SizeService()
        {
            _connectDB = new ConnectDB(); 
        }

       
        // Hàm lấy tất cả các Size
        public async Task<List<Sizes>> GetAllSizes()
        {
            var sizes = new List<Sizes>();
            var query = "SELECT Id, Size, Price FROM Size"; 

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
                                var size = new Sizes
                                {
                                    Id = reader.GetInt32(0),         
                                    Size = reader.GetString(1),      
                                    Price = reader.GetDecimal(2)     
                                };

                                sizes.Add(size); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");  
            }

            return sizes; 
        }

    }
}
