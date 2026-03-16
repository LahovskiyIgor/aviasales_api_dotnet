namespace AirlineAPI.Entity
{
    public class FlightSeat
    {
        public int Id { get; set; }
        
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        
        public string SeatNumber { get; set; }
        
        public SeatStatus Status { get; set; } = SeatStatus.Available;
        
        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }
    }
    
    public enum SeatStatus
    {
        Available = 0,    // Место свободно
        Reserved = 1,     // Место зарезервировано
        Sold = 2,         // Место продано (оплачено)
        Blocked = 3       // Место заблокировано (недоступно для выбора)
    }
}
