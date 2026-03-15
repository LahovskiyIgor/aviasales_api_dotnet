using AirlineAPI.Data;
using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

namespace AirlineAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.Include(u => u.Passenger).FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.Include(u => u.Passenger).FirstOrDefaultAsync(u => u.Id == id);
    }
}
