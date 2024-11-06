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
    }
}
