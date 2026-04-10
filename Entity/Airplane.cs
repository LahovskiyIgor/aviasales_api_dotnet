using System.Text.Json.Serialization;

namespace AirlineAPI.Entity
{
    public class Airplane
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }

        [JsonIgnore]
        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
        
        [JsonIgnore]
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
