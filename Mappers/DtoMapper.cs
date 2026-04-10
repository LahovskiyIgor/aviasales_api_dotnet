using AirlineAPI.Entity;
using AirlineAPI.DTOs;

namespace AirlineAPI.Mappers
{
    public static class DtoMapper
    {
        // Airplane mappings
        public static AirplaneDto ToDto(this Airplane airplane) => new()
        {
            Id = airplane.Id,
            Model = airplane.Model,
            Capacity = airplane.Capacity
        };

        public static AirplaneWithFlightsDto ToWithFlightsDto(this Airplane airplane) => new()
        {
            Id = airplane.Id,
            Model = airplane.Model,
            Capacity = airplane.Capacity,
            Flights = airplane.Flights?.Select(f => f.ToShortDto()).ToList() ?? new()
        };

        // Airport mappings
        public static AirportDto ToDto(this AirportEntity airport) => new()
        {
            Id = airport.Id,
            Name = airport.Name,
            Location = airport.Location
        };

        public static AirportWithFlightsDto ToWithFlightsDto(this AirportEntity airport) => new()
        {
            Id = airport.Id,
            Name = airport.Name,
            Location = airport.Location,
            DepartingFlights = airport.DepartingFlights?.Select(f => f.ToShortDto()).ToList() ?? new(),
            ArrivingFlights = airport.ArrivingFlights?.Select(f => f.ToShortDto()).ToList() ?? new()
        };

        // Flight mappings
        public static FlightShortDto ToShortDto(this Flight flight) => new()
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            DepartureAirportId = flight.DepartureAirportId,
            ArrivalAirportId = flight.ArrivalAirportId,
            AirplaneId = flight.AirplaneId
        };

        public static FlightDto ToDto(this Flight flight) => new()
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            DepartureAirportId = flight.DepartureAirportId,
            DepartureAirport = flight.DepartureAirport?.ToDto(),
            ArrivalAirportId = flight.ArrivalAirportId,
            ArrivalAirport = flight.ArrivalAirport?.ToDto(),
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            AirplaneId = flight.AirplaneId,
            Airplane = flight.Airplane?.ToDto(),
            TotalSeats = flight.Airplane?.Seats?.Count ?? 0,
            SoldTickets = flight.Tickets?.Count(t => t.BookingStatus == "Оплачен") ?? 0,
            ReservedTickets = flight.Tickets?.Count(t => t.BookingStatus == "Зарезервирован") ?? 0,
            BasePrice = flight.BasePrice
        };

        public static FlightDetailsDto ToDetailsDto(this Flight flight) => new()
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            DepartureAirportId = flight.DepartureAirportId,
            DepartureAirport = flight.DepartureAirport?.ToDto() ?? new(),
            ArrivalAirportId = flight.ArrivalAirportId,
            ArrivalAirport = flight.ArrivalAirport?.ToDto() ?? new(),
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            AirplaneId = flight.AirplaneId,
            Airplane = flight.Airplane?.ToDto() ?? new(),
            TotalSeats = flight.Airplane?.Seats?.Count ?? 0,
            SoldTickets = flight.Tickets?.Count(t => t.BookingStatus == "Оплачен") ?? 0,
            ReservedTickets = flight.Tickets?.Count(t => t.BookingStatus == "Зарезервирован") ?? 0,
            BasePrice = flight.BasePrice,
            Tickets = flight.Tickets?.Select(t => t.ToShortDto()).ToList() ?? new(),
            Seats = flight.Airplane?.Seats?.Select(s => s.ToDto()).ToList() ?? new()
        };

        // Seat mappings
        public static SeatDto ToDto(this Seat seat) => new()
        {
            Id = seat.Id,
            AirplaneId = seat.AirplaneId,
            SeatNumber = seat.SeatNumber,
            Sector = seat.Sector,
            PriceMultiplier = seat.PriceMultiplier
        };

        public static SeatWithTicketsDto ToWithTicketsDto(this Seat seat) => new()
        {
            Id = seat.Id,
            AirplaneId = seat.AirplaneId,
            SeatNumber = seat.SeatNumber,
            Sector = seat.Sector,
            PriceMultiplier = seat.PriceMultiplier,
            Tickets = seat.Tickets?.Select(t => t.ToShortDto()).ToList() ?? new()
        };

        // Ticket mappings
        public static TicketShortDto ToShortDto(this Ticket ticket) => new()
        {
            Id = ticket.Id,
            FlightId = ticket.FlightId,
            PassengerId = ticket.PassengerId,
            SeatId = ticket.SeatId,
            BookingStatus = ticket.BookingStatus,
            CalculatedPrice = ticket.Flight?.BasePrice * ticket.Seat?.PriceMultiplier ?? 0
        };

        public static TicketDto ToDto(this Ticket ticket) => new()
        {
            Id = ticket.Id,
            FlightId = ticket.FlightId,
            Flight = ticket.Flight?.ToShortDto(),
            PassengerId = ticket.PassengerId,
            Passenger = ticket.Passenger?.ToShortDto(),
            SeatId = ticket.SeatId,
            Seat = ticket.Seat?.ToDto(),
            BookingStatus = ticket.BookingStatus,
            CalculatedPrice = ticket.Flight?.BasePrice * ticket.Seat?.PriceMultiplier ?? 0
        };

        public static TicketDetailsDto ToDetailsDto(this Ticket ticket) => new()
        {
            Id = ticket.Id,
            FlightId = ticket.FlightId,
            Flight = ticket.Flight?.ToDto(),
            PassengerId = ticket.PassengerId,
            Passenger = ticket.Passenger?.ToDto(),
            SeatId = ticket.SeatId,
            Seat = ticket.Seat?.ToDto(),
            BookingStatus = ticket.BookingStatus,
            CalculatedPrice = ticket.Flight?.BasePrice * ticket.Seat?.PriceMultiplier ?? 0
        };

        // Passenger mappings
        public static PassengerShortDto ToShortDto(this Passenger passenger) => new()
        {
            Id = passenger.Id,
            FirstName = passenger.FirstName,
            LastName = passenger.LastName
        };

        public static PassengerDto ToDto(this Passenger passenger) => new()
        {
            Id = passenger.Id,
            FirstName = passenger.FirstName,
            LastName = passenger.LastName,
            Email = passenger.Email,
            Phone = passenger.Phone,
            UserId = passenger.UserId
        };

        public static PassengerDetailsDto ToDetailsDto(this Passenger passenger) => new()
        {
            Id = passenger.Id,
            FirstName = passenger.FirstName,
            LastName = passenger.LastName,
            Email = passenger.Email,
            Phone = passenger.Phone,
            UserId = passenger.UserId,
            Tickets = passenger.Tickets?.Select(t => t.ToShortDto()).ToList() ?? new()
        };

        // User mappings
        public static UserDto ToDto(this User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };

        public static UserWithPassengerDto ToWithPassengerDto(this User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
            Passenger = user.Passenger?.ToShortDto()
        };
    }
}
