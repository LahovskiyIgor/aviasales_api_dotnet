namespace AirlineAPI.Entity
{
    public class Ticket
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public Flight Flight { get; set; }

        public int PassengerId { get; set; }
        public Passenger Passenger { get; set; }

        public string BookingStatus { get; set; } = "Забронирован"; // Значение по умолчанию: Забронирован, Зарезервирован, Оплачен, Отменен
        public string SeatNumber { get; set; }
        
        public DateTime? ReservationExpiresAt { get; set; }
    }
}
