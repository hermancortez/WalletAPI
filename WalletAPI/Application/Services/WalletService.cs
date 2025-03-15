using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;

        public WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<WalletDto>> GetAllWalletsAsync()
        {
            var wallets = await _walletRepository.GetAllAsync();

            return wallets.Select(w => new WalletDto
            {
                Id = w.Id,
                DocumentId = w.DocumentId,
                Name = w.Name,
                Balance = w.Balance,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            });
        }

        public async Task<WalletDto?> GetWalletByIdAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return null;

            return new WalletDto
            {
                Id = wallet.Id,
                DocumentId = wallet.DocumentId,
                Name = wallet.Name,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        public async Task<(bool Success, string Message, WalletDto? Wallet)> CreateWalletAsync(CreateWalletDto walletDto)
        {
            if (string.IsNullOrWhiteSpace(walletDto.DocumentId) || string.IsNullOrWhiteSpace(walletDto.Name))
                return (false, "Nombre y Documento son obligatorios.", null);

            if (walletDto.InitialBalance < 0)
                return (false, "El saldo inicial debe ser positivo o cero.", null);

            var wallet = new Wallet
            {
                DocumentId = walletDto.DocumentId,
                Name = walletDto.Name,
                Balance = walletDto.InitialBalance,
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.CreateAsync(wallet);

            var walletResult = new WalletDto
            {
                Id = wallet.Id,
                DocumentId = wallet.DocumentId,
                Name = wallet.Name,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt
            };

            return (true, "Wallet creada correctamente.", walletResult);
        }

        public async Task<(bool Success, string Message)> UpdateWalletAsync(int id, UpdateWalletDto walletDto)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return (false, $"Billetera con id {id} no existe.");

            wallet.Name = walletDto.Name;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _walletRepository.UpdateAsync(wallet);

            return (true, "Wallet actualizada correctamente.");
        }

        public async Task<(bool Success, string Message)> DeleteWalletAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                return (false, $"Billetera con id {id} no existe.");

            await _walletRepository.DeleteAsync(id);

            return (true, "Wallet eliminada correctamente.");
        }

        public async Task<(bool Success, string Message)> TransferAsync(int sourceWalletId, int targetWalletId, decimal amount)
        {
            if (amount <= 0)
                return (false, "El monto debe ser mayor que cero.");

            var sourceWallet = await _walletRepository.GetByIdAsync(sourceWalletId);
            if (sourceWallet == null)
                return (false, "La billetera origen no existe.");

            if (sourceWallet.Balance < amount)
                return (false, "Saldo insuficiente.");

            var targetWallet = await _walletRepository.GetByIdAsync(targetWalletId);
            if (targetWallet == null)
                return (false, "La billetera destino no existe.");

            sourceWallet.Balance -= amount;
            sourceWallet.UpdatedAt = DateTime.UtcNow;

            targetWallet.Balance += amount;
            targetWallet.UpdatedAt = DateTime.UtcNow;

            await _walletRepository.UpdateAsync(sourceWallet);
            await _walletRepository.UpdateAsync(targetWallet);

            await _transactionRepository.AddTransactionAsync(new Transaction
            {
                WalletId = sourceWalletId,
                Amount = amount,
                Type = TransactionType.Debit,
                CreatedAt = DateTime.UtcNow
            });

            await _transactionRepository.AddTransactionAsync(new Transaction
            {
                WalletId = targetWalletId,
                Amount = amount,
                Type = TransactionType.Credit,
                CreatedAt = DateTime.UtcNow
            });

            return (true, "Transferencia realizada correctamente.");
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByWalletAsync(int walletId, DateTime? fromDate, DateTime? toDate, int page, int pageSize)
        {
            var transactions = await _transactionRepository.GetByWalletIdAsync(walletId);

            if (fromDate.HasValue)
                transactions = transactions.Where(t => t.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                transactions = transactions.Where(t => t.CreatedAt <= toDate.Value);

            return transactions
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    WalletId = t.WalletId,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    CreatedAt = t.CreatedAt
                });
        }
    }
}
