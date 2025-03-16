namespace Domain.Entities
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateTime LoginTime { get; set; }
        public bool Success { get; set; }
    }
}
