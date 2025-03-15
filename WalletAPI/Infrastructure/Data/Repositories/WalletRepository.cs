using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int walletId)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<Wallet?> GetByIdAsync(int walletId)
        {
            return await _context.Wallets.FindAsync(walletId);
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
