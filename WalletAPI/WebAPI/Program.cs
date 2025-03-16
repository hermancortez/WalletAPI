using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext EF Core (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra repositorios
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Registro de servicios
builder.Services.AddScoped<IWalletService, WalletService>();

// Habilita CORS (para solucionar el error)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// soporte para Controllers
builder.Services.AddControllers();

// documentación automática con Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Esto mostrará los errores en detalle en Swagger y Postman
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita logs detallados para capturar errores en pruebas
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

// Mapea los controladores
app.MapControllers();

app.Run();

public partial class Program { }