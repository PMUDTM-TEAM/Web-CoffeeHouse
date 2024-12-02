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

        public async Task<Orders> GetOrderById(int orderId)
        {
            string query = @"
        SELECT Id, TotalPrice, Status, PaymentStatus, PaymentMethod, Address_Id, A_Id, CreatedAt
        FROM [Order]
        WHERE Id = @OrderId";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    Orders order = null;

                    if (await reader.ReadAsync())
                    {
                        order = new Orders
                        {
                            Id = reader.GetInt32(0),
                            TotalPrice = reader.GetDecimal(1),
                            Status = reader.GetString(2),
                            PaymentStatus = reader.GetString(3),
                            PaymentMethod = reader.GetString(4),
                            Address_Id = reader.GetInt32(5),
                            A_Id = reader.GetInt32(6),
                            CreatedAt = reader.GetDateTime(7)
                        };
                    }

                    return order;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null; 
            }
        }


        public async Task<List<Orders>> GetOrdersByAccountId(int accountId)
        {
            string query = @"
        SELECT Id, TotalPrice, Status, PaymentStatus, PaymentMethod, Address_Id, A_Id, CreatedAt
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
                            TotalPrice = reader.GetDecimal(1),
                            Status = reader.GetString(2),
                            PaymentStatus = reader.GetString(3),
                            PaymentMethod = reader.GetString(4),
                            Address_Id = reader.GetInt32(5),
                            A_Id = reader.GetInt32(6),
                            CreatedAt = reader.GetDateTime(7) 
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


        public async Task<List<OrderDetails>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            string query = @"
   SELECT od.Id AS OrderDetailId, 
          od.Quantity, 
          od.TotalPrice AS Price, 
          p.Name AS ProductName, 
          p.Image AS ProductImage,
          ISNULL((
              SELECT STRING_AGG(t.Name, ',') 
              FROM Topping t
              INNER JOIN OrderTopping ot ON t.Id = ot.Topping_Id
              WHERE ot.OrderDetail_Id = od.Id
          ), '') AS Toppings,
          ISNULL((
              SELECT SUM(t.Price) 
              FROM Topping t
              INNER JOIN OrderTopping ot ON t.Id = ot.Topping_Id
              WHERE ot.OrderDetail_Id = od.Id
          ), 0) AS ToppingPrice, 
          s.Price AS SizePrice,
          s.Size AS SizeName,
            pv.Price
   FROM OrderDetail od
   INNER JOIN ProductVariant pv ON od.ProVar_Id = pv.Id
   INNER JOIN Product p ON pv.Pro_Id = p.Id
   LEFT JOIN Size s ON pv.Size_Id = s.Id 
   WHERE od.Order_Id = @Order_Id";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Order_Id", orderId);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    var orderDetailsList = new List<OrderDetails>();

                    while (await reader.ReadAsync())
                    {
                        orderDetailsList.Add(new OrderDetails
                        {
                            Id = reader.GetInt32(0),
                            Quantity = reader.GetInt32(1),
                            Price = reader.GetDecimal(2),
                            ProductName = reader.GetString(3),
                            ProductImage = reader.GetString(4),
                            Toppings = reader.IsDBNull(5) ? new List<string>() : reader.GetString(5)?.Split(',').ToList(),
                            ToppingsPrice = reader.GetDecimal(6),
                            SizePrice = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                            SizeName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8) 

                        });
                    }

                    return orderDetailsList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<OrderDetails>();
            }
        }


        public async Task<bool> CancelOrderAsync(int orderId)
        {
            string query = @"
        UPDATE [Order]
        SET Status = 'Canceled'
        WHERE Id = @OrderId AND Status = 'Pending'";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        
        public async Task<List<Orders>> GetOrdersByEmailAsync(string email)
        {
            string query = @"
        SELECT Id, TotalPrice, Status, PaymentStatus, PaymentMethod, Address_Id, A_Id, CreatedAt
        FROM [Order] o
        INNER JOIN Account a ON o.A_Id = a.Id
        WHERE a.Email = @Email";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectDB.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    var orders = new List<Orders>();

                    while (await reader.ReadAsync())
                    {
                        orders.Add(new Orders
                        {
                            Id = reader.GetInt32(0),
                            TotalPrice = reader.GetDecimal(1),
                            Status = reader.GetString(2),
                            PaymentStatus = reader.GetString(3),
                            PaymentMethod = reader.GetString(4),
                            Address_Id = reader.GetInt32(5),
                            A_Id = reader.GetInt32(6),
                            CreatedAt = reader.GetDateTime(7)
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
