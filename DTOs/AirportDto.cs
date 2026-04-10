namespace AirlineAPI.DTOs
{
    public class AirportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    public class AirportWithFlightsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public List<FlightShortDto> DepartingFlights { get; set; } = new();
        public List<FlightShortDto> ArrivingFlights { get; set; } = new();
    }
}
