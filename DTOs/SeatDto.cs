namespace AirlineAPI.DTOs
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int AirplaneId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string Sector { get; set; } = null!;
    }

    public class SeatWithTicketsDto
    {
        public int Id { get; set; }
        public int AirplaneId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string Sector { get; set; } = null!;
        public List<TicketShortDto> Tickets { get; set; } = new();
    }
}
