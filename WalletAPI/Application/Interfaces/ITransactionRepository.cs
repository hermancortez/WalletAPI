using Domain.Entities;


namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
    }
}
