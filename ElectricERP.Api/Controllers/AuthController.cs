using ElectricERP.Application.DTOs.Auth;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectricERP.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            return Ok(result);
        }

        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorDto model)
        {
            var result = await _authService.VerifyTwoFactorAsync(model);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Código 2FA inválido o token 2FA expirado."
                });
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var result = await _authService.RefreshTokenAsync(model);
            if (result == null) return BadRequest("Token inválido o expirado.");
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            await _authService.LogoutAsync(userId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var success = await _authService.ChangePasswordAsync(userId, model);
            if (!success) return BadRequest("No se pudo cambiar la contraseña.");
            return Ok(new { message = "Contraseña actualizada con éxito." });
        }

        [Authorize]
        [HttpPost("2fa/setup")]
        public async Task<IActionResult> SetupTwoFactor()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _authService.SetupTwoFactorAsync(userId);
            if (result == null) return BadRequest(new
                {
                    message = "No se pudo configurar 2FA."
                });
            return Ok(result);
        }

        [Authorize]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var success = await _authService.EnableTwoFactorAsync(userId,model.Code);
            if (!success)
            {
                return BadRequest(new{ message = "El código del autenticador es inválido." });
            }
            return Ok(new {message = "Autenticación de dos factores activada correctamente."});
        }
    }
}