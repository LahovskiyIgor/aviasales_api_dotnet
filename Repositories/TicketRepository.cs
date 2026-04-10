using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;



namespace AirlineAPI.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Ticket>> GetAllAsync() => await _context.Tickets.ToListAsync();
        
        public async Task<IEnumerable<Ticket>> GetAllWithDetailsAsync() => 
            await _context.Tickets
                .Include(t => t.Seat)
                .Include(t => t.Flight)
                .ToListAsync();
        
        public async Task<Ticket> GetByIdAsync(int id) => 
            await _context.Tickets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        
        public async Task AddAsync(Ticket ticket) { _context.Tickets.Add(ticket); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Ticket ticket) { _context.Tickets.Update(ticket); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(int id) { var entity = await _context.Tickets.FindAsync(id); if (entity != null) { _context.Tickets.Remove(entity); await _context.SaveChangesAsync(); } }
    }
}
