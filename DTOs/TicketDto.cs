namespace AirlineAPI.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public FlightShortDto? Flight { get; set; }

        public int PassengerId { get; set; }
        public PassengerShortDto? Passenger { get; set; }

        public int SeatId { get; set; }
        public SeatDto? Seat { get; set; }

        public string BookingStatus { get; set; } = "Оплачен";
    }

    public class TicketShortDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public int SeatId { get; set; }
        public string BookingStatus { get; set; } = "Оплачен";
    }

    public class TicketDetailsDto
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public FlightDto? Flight { get; set; }

        public int PassengerId { get; set; }
        public PassengerDto? Passenger { get; set; }

        public int SeatId { get; set; }
        public SeatDto? Seat { get; set; }

        public string BookingStatus { get; set; } = "Оплачен";
    }
}
