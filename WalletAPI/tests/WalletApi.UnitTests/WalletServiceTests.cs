using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace WalletApi.UnitTests;

public class WalletServiceTests
{
    private readonly WalletService _walletService;
    private readonly Mock<IWalletRepository> _walletRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;

    public WalletServiceTests()
    {
        _walletRepoMock = new Mock<IWalletRepository>();
        _transactionRepoMock = new Mock<ITransactionRepository>();

        _walletService = new WalletService(_walletRepoMock.Object, _transactionRepoMock.Object);
    }



    [Fact]
    public async Task TransferAsync_ShouldFail_WhenInsufficientBalance()
    {
        // Arrange
        int sourceWalletId = 1;
        int targetWalletId = 2;
        decimal amount = 150m;

        var sourceWallet = new Wallet { Id = sourceWalletId, Balance = 100m };
        var targetWallet = new Wallet { Id = targetWalletId, Balance = 50m };

        _walletRepoMock.Setup(repo => repo.GetByIdAsync(sourceWalletId)).ReturnsAsync(sourceWallet);
        _walletRepoMock.Setup(repo => repo.GetByIdAsync(targetWalletId)).ReturnsAsync(targetWallet);

        // Act
        var result = await _walletService.TransferAsync(sourceWalletId, targetWalletId, amount);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Saldo insuficiente.", result.Message);
    }

    [Fact]
    public async Task TransferAsync_ShouldSucceed_WhenBalanceIsSufficient()
    {
        // Arrange
        int sourceWalletId = 1;
        int targetWalletId = 2;
        decimal amount = 50m;

        var sourceWallet = new Wallet { Id = sourceWalletId, Balance = 100m };
        var targetWallet = new Wallet { Id = targetWalletId, Balance = 50m };

        _walletRepoMock.Setup(repo => repo.GetByIdAsync(sourceWalletId)).ReturnsAsync(sourceWallet);
        _walletRepoMock.Setup(repo => repo.GetByIdAsync(targetWalletId)).ReturnsAsync(targetWallet);

        // Act
        var result = await _walletService.TransferAsync(sourceWalletId, targetWalletId, amount);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Transferencia realizada correctamente.", result.Message);
        _walletRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Wallet>()), Times.Exactly(2));
        _transactionRepoMock.Verify(repo => repo.AddTransactionAsync(It.IsAny<Transaction>()), Times.Exactly(2));
    }
}
