namespace AirlineAPI.DTOs
{
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = null!;

        public int DepartureAirportId { get; set; }
        public AirportDto? DepartureAirport { get; set; }

        public int ArrivalAirportId { get; set; }
        public AirportDto? ArrivalAirport { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int AirplaneId { get; set; }
        public AirplaneDto? Airplane { get; set; }

        public int TotalSeats { get; set; }
        public int SoldTickets { get; set; }
        public int ReservedTickets { get; set; }
    }

    public class FlightShortDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int AirplaneId { get; set; }
    }

    public class FlightDetailsDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = null!;

        public int DepartureAirportId { get; set; }
        public AirportDto DepartureAirport { get; set; } = null!;

        public int ArrivalAirportId { get; set; }
        public AirportDto ArrivalAirport { get; set; } = null!;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int AirplaneId { get; set; }
        public AirplaneDto Airplane { get; set; } = null!;

        public int TotalSeats { get; set; }
        public int SoldTickets { get; set; }
        public int ReservedTickets { get; set; }

        public List<TicketShortDto> Tickets { get; set; } = new();
    }
}
