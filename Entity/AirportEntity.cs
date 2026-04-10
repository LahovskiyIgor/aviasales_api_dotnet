using System.Text.Json.Serialization;

namespace AirlineAPI.Entity
{
    public class AirportEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        [JsonIgnore]
        public ICollection<Flight> DepartingFlights { get; set; }
        
        [JsonIgnore]
        public ICollection<Flight> ArrivingFlights { get; set; }
    }
}
