using Application.Dtos;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(UserDto user);
        void LogLoginAttempt(string username, bool success);
        Task<(bool Success, string Message)> RegisterUser(RegisterUserDto userDto);
        Task<UserDto> ValidateUserCredentials(string username, string password);
    }
}
