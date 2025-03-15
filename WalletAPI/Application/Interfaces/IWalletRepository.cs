using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet?> GetByIdAsync(int walletId);
        Task CreateAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(int walletId);
    }
}
