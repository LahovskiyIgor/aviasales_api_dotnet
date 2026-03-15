namespace AirlineAPI.Entity
{
    public class Passenger
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        
        // Внешний ключ на таблицу Users
        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
