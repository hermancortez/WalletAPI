namespace Application.Dtos
{
    public class TransferDto
    {
        public int SourceWalletId { get; set; }
        public int TargetWalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
