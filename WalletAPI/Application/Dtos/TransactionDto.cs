namespace Application.Dtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
