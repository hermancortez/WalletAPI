using Application.Dtos;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace WalletApi.IntegrationTests;

public class WalletControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WalletControllerTests(WebApplicationFactory<Program> factory)
    {

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection"); 

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Eliminar el DbContext existente si está registrado
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Registrar DbContext con SQL Server
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Aplicar migraciones y limpiar la BD antes de cada prueba
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate(); // Aplica migraciones solo si hay cambios
                }

            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetWallets_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/wallet");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateWallet_ReturnsCreated()
    {
        // Arrange
        var newWallet = new CreateWalletDto
        {
            DocumentId = "12345678-9",
            Name = "Test Wallet",
            InitialBalance = 100.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/wallet", newWallet);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}