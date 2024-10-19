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

        // Method to get order details by order ID
        public async Task<List<OrderDetails>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            string query = @"
        SELECT od.Id AS OrderDetailId, od.Quantity, od.TotalPrice AS Price, 
               p.Name AS ProductName, p.Image AS ProductImage,
               ISNULL((
                   SELECT STRING_AGG(t.Name, ',') 
                   FROM Topping t
                   INNER JOIN OrderTopping ot ON t.Id = ot.Topping_Id
                   WHERE ot.OrderDetail_Id = od.Id
               ), '') AS Toppings
        FROM OrderDetail od
        INNER JOIN ProductVariant pv ON od.ProVar_Id = pv.Id
        INNER JOIN Product p ON pv.Pro_Id = p.Id
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
                            Toppings = reader.IsDBNull(5) ? new List<string>() : reader.GetString(5)?.Split(',').ToList()
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

    }
}
