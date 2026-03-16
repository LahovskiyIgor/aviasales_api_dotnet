namespace AirlineAPI.Entity
{
    public class FlightSeat
    {
        public int Id { get; set; }
        
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        
        public string SeatNumber { get; set; } = string.Empty;
        
        public bool IsAvailable { get; set; } = true;
        
        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
