using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador para la autenticación y registro de usuarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor del controlador de autenticación.
        /// </summary>
        /// <param name="authService">Servicio de autenticación</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        /// <summary>
        /// Autentica a un usuario y genera un token JWT.
        /// </summary>
        /// <param name="loginDto">Credenciales del usuario.</param>
        /// <returns>Token de acceso si las credenciales son correctas.</returns>
        /// <response code="200">Devuelve el token de autenticación.</response>
        /// <response code="401">Credenciales incorrectas.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.ValidateUserCredentials(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }


        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="userDto">Datos del usuario a registrar.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        /// <response code="200">Usuario registrado correctamente.</response>
        /// <response code="400">Error en la creación del usuario.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto userDto)
        {
            var result = await _authService.RegisterUser(userDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }


}
