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

        

    }
}
