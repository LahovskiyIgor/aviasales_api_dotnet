namespace AirlineAPI.Entity
{
    public class AirportEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Flight> DepartingFlights { get; set; }
        public ICollection<Flight> ArrivingFlights { get; set; }
    }
}
