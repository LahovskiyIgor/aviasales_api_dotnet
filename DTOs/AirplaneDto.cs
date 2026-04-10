namespace AirlineAPI.DTOs
{
    public class AirplaneDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public int Capacity { get; set; }
    }

    public class AirplaneWithFlightsDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = null!;
        public int Capacity { get; set; }
        public List<FlightShortDto> Flights { get; set; } = new();
    }
}
