namespace AirlineAPI.Entity
{
    public class Airplane
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();

    }
}
