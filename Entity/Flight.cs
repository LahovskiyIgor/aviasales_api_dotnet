namespace AirlineAPI.Entity
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }

        public int DepartureAirportId { get; set; }
        public AirportEntity DepartureAirport { get; set; }

        public int ArrivalAirportId { get; set; }
        public AirportEntity ArrivalAirport { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int AirplaneId { get; set; }
        public Airplane Airplane { get; set; }

        public int TotalSeats { get; set; }
        public int SoldTickets { get; set; }
        public int ReservedTickets { get; set; }

        public decimal BasePrice { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
