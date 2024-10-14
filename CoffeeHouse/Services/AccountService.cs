using CoffeeHouse.Identity;
using CoffeeHouse.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

      
    }
}
