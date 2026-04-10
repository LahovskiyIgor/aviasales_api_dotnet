using System.Text.Json.Serialization;

namespace AirlineAPI.Entity
{
    public class Ticket
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        
        [JsonIgnore]
        public Flight Flight { get; set; }

        public int PassengerId { get; set; }
        
        [JsonIgnore]
        public Passenger Passenger { get; set; }

        public int SeatId { get; set; }
        
        [JsonIgnore]
        public Seat Seat { get; set; }

        public string BookingStatus { get; set; } = "Оплачен"; // Значение по умолчанию: Зарезервирован, Оплачен, Отменен
    }
}
