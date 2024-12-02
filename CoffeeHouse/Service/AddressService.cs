using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class AddressService
    {
        private readonly ConnectDB _connectDB;

        public AddressService()
        {
            _connectDB = new ConnectDB();
        }

        // Hàm lấy Id max của address của Account truyền vào
        public async Task<int> GetAddressIdMaxByAIdAsync(int A_Id)
        {
            var query = @"
                    SELECT COALESCE(MAX(Id), 0) AS MaxId
                    FROM Address
                    WHERE A_Id = @A_Id";  

            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  // Mở kết nối

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số A_Id
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                        // Thực thi câu lệnh và lấy kết quả
                        var result = await command.ExecuteScalarAsync();
                        return (result != DBNull.Value) ? Convert.ToInt32(result) : 0;  
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}"); 
                return 0;  
            }
        }


        // Thêm đia chỉ
        public async Task AddToAddressAsync(Addresses address)
        {
            // Chuỗi truy vấn SQL để chèn vào bảng Addresses
            string query = @"
                    INSERT INTO Address (Phone, Ward, Address, District, A_Id)
                    VALUES (@Phone, @Ward, @Address, @District, @A_Id)";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Thêm các tham số
                        command.Parameters.AddWithValue("@Phone", address.Phone);
                        command.Parameters.AddWithValue("@Ward", address.Ward);
                        command.Parameters.AddWithValue("@Address", address.Address);
                        command.Parameters.AddWithValue("@District", address.District);
                        command.Parameters.AddWithValue("@A_Id", address.A_ID);

                        // Thực thi câu lệnh
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
