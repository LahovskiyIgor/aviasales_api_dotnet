namespace AirlineAPI.DTOs
{
    public class PassengerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int UserId { get; set; }
    }

    public class PassengerShortDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public class PassengerDetailsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int UserId { get; set; }
        public List<TicketShortDto> Tickets { get; set; } = new();
    }

    public class UpdatePassengerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
