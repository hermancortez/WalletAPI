using Application.Dtos;

namespace Application.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletDto>> GetAllWalletsAsync();
        Task<WalletDto?> GetWalletByIdAsync(int id);
        Task<(bool Success, string Message, WalletDto? Wallet)> CreateWalletAsync(CreateWalletDto walletDto);
        Task<(bool Success, string Message)> UpdateWalletAsync(int id, UpdateWalletDto walletDto);
        Task<(bool Success, string Message)> DeleteWalletAsync(int id);
        Task<(bool Success, string Message)> TransferAsync(int sourceWalletId, int targetWalletId, decimal amount);
        Task<IEnumerable<TransactionDto>> GetTransactionsByWalletAsync(int walletId, DateTime? fromDate, DateTime? toDate, int page, int pageSize);

    }
}
