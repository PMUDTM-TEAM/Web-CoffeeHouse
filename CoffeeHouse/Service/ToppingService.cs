using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace CoffeeHouse.Service
{
    public class ToppingService
    {
        private readonly ConnectDB _connectDB;

        public ToppingService()
        {
            _connectDB = new ConnectDB();  // Sử dụng ConnectDB để lấy chuỗi kết nối
        }

        public async Task<List<Topping>> GetAllToppings()
        {
            var toppings = new List<Topping>();
            var query = "SELECT Id, Name, Price FROM Topping";  // Giả sử bảng trong SQL Server tên là Toppings

            // Lấy chuỗi kết nối từ ConnectDB
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
                                var topping = new Topping
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetDecimal(2)
                                };
                                toppings.Add(topping);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");
            }

            return toppings;
        }
    }
}
