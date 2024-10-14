using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CoffeeHouse.Service
{
    public class ConnectDB
    {
        private readonly string _connectionString = "Server=DESKTOP-741DDSM\\DIEN;Database=DB_CF;User Id=sa;Password=123;";

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
