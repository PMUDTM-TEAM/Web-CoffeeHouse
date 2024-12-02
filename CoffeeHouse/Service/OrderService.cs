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

    }
}
