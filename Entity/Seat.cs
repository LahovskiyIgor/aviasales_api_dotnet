namespace AirlineAPI.Entity
{
    public class Seat
    {
        public int Id { get; set; }
        public int AirplaneId { get; set; }
        public Airplane Airplane { get; set; }

        public string SeatNumber { get; set; } // Например, "12A", "1B"
        public string Sector { get; set; }     // "Эконом", "Бизнес"

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
