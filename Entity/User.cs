using System.Text.Json.Serialization;

namespace AirlineAPI.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        
        // Связь с пассажиром (один пользователь может иметь одного пассажира)
        
        [JsonIgnore]
        public Passenger? Passenger { get; set; }
    }

}
