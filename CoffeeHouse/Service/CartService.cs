using CoffeeHouse.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace CoffeeHouse.Service
{
    public class CartService
    {
        private readonly ConnectDB _connectDB;

        public CartService()
        {
            _connectDB = new ConnectDB();
        }

        // Hàm xóa tất cả các Cart ra khỏi Cart bởi A_Id
        public async Task DeleteCartsByAIdAsync(int A_Id)
        {
            var query = @"
                        DELETE FROM Cart
                        WHERE A_Id = @A_Id";  

            var connectionString = _connectDB.GetConnectionString(); 

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                       
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                        
                        await command.ExecuteNonQueryAsync();  
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  
            }
        }


        // Hàm lấy tất cả các Cart dựa trên A_Id
        public async Task<List<int>> getAllCartIdByAId(int A_Id)
        {
            var query = @"
                        SELECT Id
                        FROM Cart
                        WHERE A_Id = @A_Id";  

            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                var cartIds = new List<int>(); 

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                       
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())  
                            {
                               
                                var cartId = reader.GetInt32(0);  
                                cartIds.Add(cartId);
                            }
                        }
                    }
                }

                return cartIds;  
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}"); 
                return new List<int>();  
            }
        }


        // Hàm xóa các liên kết topping của cart vs topping dựa vào cart Id
        public async Task<int> RemoveCartToppingByCartIdAsync(int cartId)
        {
            string query = @"
                            DELETE FROM CartTopping
                            WHERE Cart_Id = @Cart_Id";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Cart_Id", cartId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0 ? 1 : 0; // Trả về 1 nếu xóa thành công, 0 nếu không
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi xảy ra
            }
        }


        // Hàm xóa CartId nhờ vào Id truyền vào 
        public async Task<int> RemoveFromCartByIdAsync(int Id)
        {
            string query = @"
            DELETE FROM Cart
            WHERE Id = @Id";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0 ? 1 : 0; // Trả về 1 nếu xóa thành công, 0 nếu không
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi xảy ra
            }
        }


        // Đếm các cart dựa vào A_Id
        public async Task<int> CountCartByAIdAsync(int A_Id)
        {
            // Truy vấn SQL để đếm số lượng Cart theo A_Id
            string query = @"
                    SELECT COUNT(*) AS cartCount
                    FROM Cart
                    WHERE A_Id = @A_Id";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số @A_Id vào câu lệnh truy vấn
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                        // Thực hiện truy vấn và lấy giá trị đếm
                        var count = (int)await command.ExecuteScalarAsync();
                        return count;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 nếu có lỗi xảy ra
            }
        }


        // Hàm lấy tất cả các Cart dựa vào A_Id và các topping của cart đó
        public async Task<List<Cart>> GetAllCartByIdAsync(int A_Id)
        {
            // Truy vấn SQL lấy tất cả các Cart theo A_Id và danh sách Topping_Id từ CartTopping
            string query = @"
                            SELECT c.Id, c.ProVar_Id, c.Quantity, c.TotalPrice, ct.Topping_Id
                            FROM Cart c
                            LEFT JOIN CartTopping ct ON c.Id = ct.Cart_Id
                            WHERE c.A_Id = @A_Id
                            ORDER BY c.Id
                        ";

            // Chuẩn bị danh sách Cart
            var carts = new List<Cart>();
            var cartsDictionary = new Dictionary<int, Cart>();

            try
            {
                // Kết nối cơ sở dữ liệu
                var connectionString = _connectDB.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int cartId = reader.GetInt32(reader.GetOrdinal("Id"));

                                // Kiểm tra nếu Cart chưa tồn tại trong từ điển, tạo mới và thêm vào danh sách
                                if (!cartsDictionary.TryGetValue(cartId, out Cart cart))
                                {
                                    cart = new Cart
                                    {
                                        Id = cartId,
                                        A_Id = A_Id,
                                        Provar_Id = reader.GetInt32(reader.GetOrdinal("ProVar_Id")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                        TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                                        Topping_Id = new List<int>()
                                    };
                                    cartsDictionary[cartId] = cart;
                                    carts.Add(cart);
                                }

                                // Thêm Topping_Id vào danh sách Topping_Id của Cart nếu có
                                if (!reader.IsDBNull(reader.GetOrdinal("Topping_Id")))
                                {
                                    cart.Topping_Id.Add(reader.GetInt32(reader.GetOrdinal("Topping_Id")));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return carts;
        }


        // Đếm các cart dựa vào A_Id
        public async Task<int> countCartByAId(int A_Id)
        {
            // Truy vấn SQL để đếm số lượng Cart theo A_Id
            string query = @"
                                SELECT COUNT(*) AS cartCount
                                FROM Cart
                                WHERE A_Id = @A_Id
                            ";

            // Lấy chuỗi kết nối từ _connectDB
            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@A_Id", A_Id);

                        var count = (int)await command.ExecuteScalarAsync();
                        return count;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0; // Trả về 0 trong trường hợp có lỗi
            }
        }


        // Thêm vào bảng CartTopping
        public async Task addToCartTopping(int cartId, int toppingId)
        {
            string query = @"
                            INSERT INTO CartTopping (Cart_Id, Topping_Id)
                            VALUES (@Cart_Id, @Topping_Id)";

            var connectionString = _connectDB.GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Cart_Id", cartId);
                        command.Parameters.AddWithValue("@Topping_Id", toppingId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to CartTopping: {ex.Message}");
            }
        }

        // Hàm lấy CartId lớn nhất dựa trên A_Id
        public async Task<int> GetCartIdMaxByAIdAsync(int A_Id)
        {
            var query = @"
                SELECT COALESCE(MAX(Id), 0) AS MaxId
                FROM Cart
                WHERE A_Id = @A_Id";  

            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

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

        // Hàm thêm một bản ghi vào bảng Cart
        public async Task AddToCartAsync(Cart cart)
        {
            var query = @"
                INSERT INTO Cart (A_Id, Quantity, Provar_Id, TotalPrice) 
                VALUES (@A_Id, @Quantity, @Provar_Id, @TotalPrice)"; 

            var connectionString = _connectDB.GetConnectionString();  

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();  

                    using (var command = new SqlCommand(query, connection))
                    {
                       
                        command.Parameters.AddWithValue("@A_Id", cart.A_Id);
                        command.Parameters.AddWithValue("@Quantity", cart.Quantity);
                        command.Parameters.AddWithValue("@Provar_Id", cart.Provar_Id);
                        command.Parameters.AddWithValue("@TotalPrice", cart.TotalPrice);

                       
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");  
            }
        }

    }
}
