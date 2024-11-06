using CoffeeHouse.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class OrderService
    {
        private readonly ConnectDB _connectDB;

        public OrderService()
        {
            _connectDB = new ConnectDB();
        }

        public async Task<int> GetOrderDetailIdMaxByOrderIdAsync(int Order_Id)
        {
            var query = @"
                SELECT COALESCE(MAX(Id), 0) AS MaxId
                FROM OrderDetail
                WHERE Order_Id = @Order_Id"; 

            var connectionString = _connectDB.GetConnectionString(); 

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@Order_Id", Order_Id);

                       
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


        // Hàm thêm vào orderDetail
        public async Task<int> addToOrderDetail(OrderDetails orderDetail)
        {
            
            string query = @"
                    INSERT INTO OrderDetail (ProVar_Id, Quantity, TotalPrice, Order_Id)
                    VALUES (@ProVar_Id, @Quantity, @TotalPrice, @Order_Id);
                    SELECT SCOPE_IDENTITY();"; 

            var connectionString = _connectDB.GetConnectionString(); 

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(); 

                    using (var command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@ProVar_Id", orderDetail.Provar_Id);
                        command.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                        command.Parameters.AddWithValue("@TotalPrice", orderDetail.Price);
                        command.Parameters.AddWithValue("@Order_Id", orderDetail.Order_Id);

                       
                        var result = await command.ExecuteScalarAsync();

                        return result != DBNull.Value ? Convert.ToInt32(result) : 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to OrderDetail: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi
            }
        }



        // Hàm lấy order id lớn nhất dựa vào account truyền vào
        public async Task<int> GetOrderIdMaxByAIdAsync(int A_Id)
        {
            var query = @"
                    SELECT COALESCE(MAX(Id), 0) AS MaxId
                    FROM [Order]
                    WHERE A_Id = @A_Id";  // Truy vấn SQL để lấy OrderId lớn nhất theo A_Id

            var connectionString = _connectDB.GetConnectionString();  // Lấy chuỗi kết nối

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
                        return (result != DBNull.Value) ? Convert.ToInt32(result) : 0;  // Trả về giá trị hoặc 0 nếu không có
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  // Xử lý lỗi
                return 0;  // Trả về 0 nếu có lỗi
            }
        }


        // Hàm thêm vào OrderTopping
        public async Task AddToOrderToppingAsync(int orderDetailId, int toppingId)
        {
            string query = @"
                    INSERT INTO OrderTopping (OrderDetail_Id, Topping_Id)
                    VALUES (@OrderDetail_Id, @Topping_Id)";

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderDetail_Id", orderDetailId);
                        command.Parameters.AddWithValue("@Topping_Id", toppingId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to OrderTopping: {ex.Message}");
            }
        }

        // Hàm thêm mới vào order
        public async Task AddToOrderAsync(Orders order)
        {
            string query = @"
                    INSERT INTO [Order] (Date, Status, TotalPrice, Address_Id, A_Id)
                    VALUES (@Date, @Status, @TotalPrice, @Address_Id, @A_Id)";

            var connectionString = _connectDB.GetConnectionString();  // Lấy chuỗi kết nối từ _connectDB

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Thêm các tham số vào câu lệnh SQL
                        command.Parameters.AddWithValue("@Date", order.Date);
                        command.Parameters.AddWithValue("@Status", order.Status);
                        command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                        command.Parameters.AddWithValue("@Address_Id", order.Address_Id);
                        command.Parameters.AddWithValue("@A_Id", order.A_Id);

                        // Thực thi câu lệnh SQL
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        // Method to get the maximum Order ID from SQL Server
        public async Task<int> GetMaxIdOrder()
        {
            string query = "SELECT COALESCE(MAX(Id), 0) AS maxId FROM [Order]";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Return 0 in case of an error
            }
        }

        // Method to get the maximum OrderDetail ID from SQL Server
        public async Task<int> GetMaxIdOrderDetail()
        {
            string query = "SELECT COALESCE(MAX(Id), 0) AS maxId FROM OrderDetail";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Return 0 in case of an error
            }
        }

        // Method to insert an order into the database
        public async Task AddToOrder(Orders order)
        {
            string query = @"
                INSERT INTO [Order] (Date, Status, TotalPrice, Address_Id, A_Id)
                VALUES (@Date, @Status, @TotalPrice, @Address_Id, @A_Id)";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Date", order.Date);
                    cmd.Parameters.AddWithValue("@Status", order.Status);
                    cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    cmd.Parameters.AddWithValue("@Address_Id", order.Address_Id);
                    cmd.Parameters.AddWithValue("@A_Id", order.A_Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Method to insert order details into the database
        public async Task AddToOrderDetail(OrderDetails orderDetail)
        {
            string query = @"
                INSERT INTO OrderDetail (Quantity, TotalPrice, Order_Id, ProVar_Id)
                VALUES (@Quantity, @TotalPrice, @Order_Id, @ProVar_Id)";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                    cmd.Parameters.AddWithValue("@TotalPrice", orderDetail.Price);
                    cmd.Parameters.AddWithValue("@Order_Id", orderDetail.Order_Id);
                    cmd.Parameters.AddWithValue("@ProVar_Id", orderDetail.Provar_Id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        

        // Method to get order by order ID
        public async Task<Orders> GetOrderById(int orderId)
        {
            string query = @"
                SELECT Id, Date, Status, TotalPrice, Address_Id, A_Id
                FROM [Order]
                WHERE Id = @Order_Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Order_Id", orderId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        return new Orders
                        {
                            Id = reader.GetInt32(0),
                            Date = reader.GetDateTime(1),
                            Status = reader.GetString(2),
                            TotalPrice = reader.GetDecimal(3),
                            Address_Id = reader.GetInt32(4),
                            A_Id = reader.GetInt32(5)
                        };
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        // Method to get orders by account ID
        public async Task<List<Orders>> GetOrdersByAccountId(int accountId)
        {
            string query = @"
                SELECT Id, OrderDate, Status, TotalPrice, Address_Id, A_Id
                FROM [Order]
                WHERE A_Id = @A_Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@A_Id", accountId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    var orders = new List<Orders>();

                    while (await reader.ReadAsync())
                    {
                        orders.Add(new Orders
                        {
                            Id = reader.GetInt32(0),
                            Date = reader.GetDateTime(1),
                            Status = reader.GetString(2),
                            TotalPrice = reader.GetDecimal(3),
                            Address_Id = reader.GetInt32(4),
                            A_Id = reader.GetInt32(5)
                        });
                    }

                    return orders;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<Orders>();
            }
        }

        

    }
}
