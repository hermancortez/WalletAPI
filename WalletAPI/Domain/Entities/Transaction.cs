namespace Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Wallet? Wallet { get; set; }
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }
}
