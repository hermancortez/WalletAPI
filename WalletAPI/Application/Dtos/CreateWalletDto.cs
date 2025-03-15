namespace Application.Dtos
{
    public class CreateWalletDto
    {
        public string? DocumentId { get; set; }
        public string? Name { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
