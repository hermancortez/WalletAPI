using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        public string GenerateJwtToken(UserDto user)
        {

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("La clave JWT no está definida en appsettings.json");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? "Unknown"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email  ?? "Unknown"),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void LogLoginAttempt(string username, bool success)
        {
            var loginHistory = new LoginHistory
            {
                Username = username,
                LoginTime = DateTime.UtcNow,
                Success = success
            };

            _userRepository.AddLoginHistory(loginHistory);
        }

        public async Task<UserDto> ValidateUserCredentials(string username, string password)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                LogLoginAttempt(username, false);
                return null!;
            }

            LogLoginAttempt(username, true);

            return new UserDto
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<(bool Success, string Message)> RegisterUser(RegisterUserDto userDto)
        {

            var existingUser = await _userRepository.GetUserByUsername(userDto.Username);
            if (existingUser != null)
                return (false, "El usuario ya existe.");

            // Encriptar la contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Crear la entidad usuario
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                Role = "User" 
            };

            // Guardar el usuario en la BD
            await _userRepository.AddUser(user);

            return (true, "Usuario registrado correctamente.");
        }

    }
}
