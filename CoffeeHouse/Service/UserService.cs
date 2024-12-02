using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CoffeeHouse.Models;
using BCrypt.Net;
using CoffeeHouse.Service;

namespace CoffeeHouse.Service
{
    public class UserService
    {
        private readonly ConnectDB _connectDB;

        public UserService()
        {
            _connectDB = new ConnectDB(); 
        }

        public async Task<int> GetMaxIdAsync()
        {
            using (var connection = new SqlConnection(_connectDB.GetConnectionString()))
            {
                await connection.OpenAsync();
                var query = "SELECT ISNULL(MAX(Id), 0) AS maxId FROM Account";
                using (var command = new SqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return (int)result;
                }
            }
        }

        public async Task<Accounts> GetUserByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectDB.GetConnectionString()))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Account WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return new Accounts
                            {
                                A_ID = (int)reader["Id"],
                                A_NAME = reader["Name"].ToString(),
                                A_EMAIL = reader["Email"].ToString(),
                                A_PASSWORD = reader["Password"].ToString(),
                                ROLE_ID = (int)reader["Role_Id"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<Accounts> RegisterAsync(Register model)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            using (var connection = new SqlConnection(_connectDB.GetConnectionString()))
            {
                await connection.OpenAsync();
                var query = @"
            INSERT INTO Account (Name, Email, Password, Role_Id)
            VALUES (@Name, @Email, @Password, 2);
            SELECT SCOPE_IDENTITY();";  // Lấy ID vừa tạo

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    var newUserId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    return new Accounts
                    {
                        A_ID = newUserId,
                        A_NAME = model.Name,
                        A_EMAIL = model.Email,
                        A_PASSWORD = hashedPassword,
                        ROLE_ID = 2 // Role mặc định
                    };
                }
            }
        }



        public async Task<Accounts> LoginAsync(string email, string password)
        {
            using (var connection = new SqlConnection(_connectDB.GetConnectionString()))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Account WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var storedPassword = reader["Password"].ToString();
                            if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                            {
                                return new Accounts
                                {
                                    A_ID = (int)reader["Id"],
                                    A_NAME = reader["Name"].ToString(),
                                    A_EMAIL = reader["Email"].ToString(),
                                    A_PASSWORD = storedPassword,
                                    ROLE_ID = (int)reader["Role_Id"]
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
