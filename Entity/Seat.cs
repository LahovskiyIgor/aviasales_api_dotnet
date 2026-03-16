namespace AirlineAPI.Entity
{
    public class Seat
    {
        public int Id { get; set; }
        
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        
        public string SeatNumber { get; set; }
        
        public string Class { get; set; } = "Economy"; // Economy, Business, First
        
        public bool IsAvailable { get; set; } = true;
    }
}
