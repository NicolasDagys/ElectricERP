using ElectricERP.Application.DTOs.Auth;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ElectricERP.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IConfiguration configuration,UserManager<ApplicationUser> userManager /*, SignInManager<ApplicationUser> signInManager*/)
        {
            _configuration = configuration;
            _userManager = userManager;
            //_signInManager = signInManager;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto model)
        {
            var usuario = await _userManager.FindByEmailAsync(model.Email);
            if (usuario == null) return null;
            if (!usuario.Activo)  throw new Exception("El usuario se encuentra inactivo.");
            var passwordCorrecta = await _userManager.CheckPasswordAsync(usuario, model.Password);
            if (!passwordCorrecta) return null;
            var roles = await _userManager.GetRolesAsync(usuario);

            // =========================================================
            // 2FA
            // =========================================================

            if (usuario.TwoFactorEnabled)
            {
                var twoFactorToken = GenerateTwoFactorToken(usuario);
                return new AuthResponseDto
                {
                    Token = string.Empty,
                    RefreshToken = string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? string.Empty,
                    RequiresTwoFactor = true,
                    TwoFactorToken = twoFactorToken
                };
            }

            // =========================================================
            // LOGIN NORMAL SIN 2FA
            // =========================================================

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id),
        new Claim(ClaimTypes.Name, usuario.Email ?? string.Empty),
        new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateToken(authClaims);
            var refreshToken = await CreateRefreshTokenAsync(usuario);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = usuario.Email ?? string.Empty,
                Rol = roles.FirstOrDefault() ?? string.Empty,
                RequiresTwoFactor = false,
                TwoFactorToken = null
            };
        }

        private string GenerateTwoFactorToken(ApplicationUser usuario)
        {
            var secret = _configuration["JwtSettings:Secret"] ?? "ClavePorDefectoSuperSecreta123456";
            var authSignerKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id),
        new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
        // Indica que este JWT solamente sirve para completar 2FA
        new Claim("purpose", "2fa")
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(authSignerKey,SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto model)
        {
            if (string.IsNullOrWhiteSpace(model.RefreshToken))  return null;
            var refreshTokenHash = HashRefreshToken(model.RefreshToken);
            var usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash);
            if (usuario == null) return null;
            if (!usuario.Activo) return null;
            if (!usuario.RefreshTokenExpiryTime.HasValue) return null;
            if (usuario.RefreshTokenExpiryTime.Value <= DateTime.UtcNow)  return null;
            var roles = await _userManager.GetRolesAsync(usuario);
            var authClaims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            usuario.Id),

        new Claim(
            ClaimTypes.Name,
            usuario.Email ?? string.Empty),

        new Claim(
            ClaimTypes.Email,
            usuario.Email ?? string.Empty),

        new Claim(
            JwtRegisteredClaimNames.Jti,
            Guid.NewGuid().ToString())
    };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = GenerateToken(authClaims);
            var newRefreshToken = await CreateRefreshTokenAsync(usuario);
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = newRefreshToken,
                Email = usuario.Email ?? string.Empty,
                Rol = roles.FirstOrDefault() ?? string.Empty,
                RequiresTwoFactor = false,
                TwoFactorToken = null
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto model)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)  return false;
            if (!usuario.Activo)  return false;
            var result = await _userManager.ChangePasswordAsync(usuario, model.CurrentPassword, model.NewPassword);
            return result.Succeeded;
        }

        private string GenerateToken(List<Claim> claims)
        {
            var secret = _configuration["JwtSettings:Secret"] ?? "ClavePorDefectoSuperSecreta123456";
            var authSignerKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var durationString =_configuration["JwtSettings:DurationInMinutes"] ?? "60";
            var duration = double.Parse(durationString);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(duration),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(authSignerKey, SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<AuthResponseDto?> VerifyTwoFactorAsync(VerifyTwoFactorDto model)
        {
            if (string.IsNullOrWhiteSpace(model.TwoFactorToken)) return null;
            if (string.IsNullOrWhiteSpace(model.Code)) return null;

            // =========================================================
            // Validar JWT temporal
            // =========================================================

            var principal = ValidateTwoFactorToken(model.TwoFactorToken);
            if (principal == null)  return null;
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return null;
            if (!usuario.Activo) return null;
            if (!usuario.TwoFactorEnabled) return null;

            // =========================================================
            // Validar código TOTP
            // =========================================================

            // -- TS --
            Console.WriteLine("========== DEBUG 2FA ==========");
            Console.WriteLine($"Usuario: {usuario.Email}");
            Console.WriteLine($"TwoFactorEnabled: {usuario.TwoFactorEnabled}");
            Console.WriteLine($"Code recibido: [{model.Code}]");
            Console.WriteLine($"Code length: {model.Code?.Length}");
            Console.WriteLine("================================");

            // -- TS --
            var serverGeneratedCode = await _userManager.GenerateTwoFactorTokenAsync(usuario,TokenOptions.DefaultAuthenticatorProvider);

            Console.WriteLine("========== DEBUG TOTP ==========");
            Console.WriteLine($"Código recibido: [{model.Code}]");
            Console.WriteLine($"Código generado por Identity: [{serverGeneratedCode}]");
            Console.WriteLine($"Longitud recibido: {model.Code?.Length}");
            Console.WriteLine($"Longitud servidor: {serverGeneratedCode?.Length}");
            Console.WriteLine("================================");

            var codeValid = await _userManager.VerifyTwoFactorTokenAsync(usuario,TokenOptions.DefaultAuthenticatorProvider,model.Code);

            Console.WriteLine($"Resultado VerifyTwoFactorTokenAsync: {codeValid}");
            Console.WriteLine("================================");

            if (!codeValid) return null;

            // -- HATSTA ACA --

            // =========================================================
            // Código correcto → generar JWT real
            // =========================================================

            var roles = await _userManager.GetRolesAsync(usuario);
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id),
        new Claim(ClaimTypes.Name, usuario.Email ?? string.Empty),
        new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = GenerateToken(authClaims);
            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = "TEMPORAL",
                Email = usuario.Email ?? string.Empty,
                Rol = roles.FirstOrDefault() ?? string.Empty,
                RequiresTwoFactor = false,
                TwoFactorToken = null
            };
        }

        private ClaimsPrincipal? ValidateTwoFactorToken(
    string token)
        {
            var secret = _configuration["JwtSettings:Secret"] ?? "ClavePorDefectoSuperSecreta123456";
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["JwtSettings:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    },
                    out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken) return null;
                var purpose = principal.FindFirst("purpose")?.Value;
                if (purpose != "2fa") return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TwoFactorSetupDto?> SetupTwoFactorAsync(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return null;
            if (!usuario.Activo) return null;
            var key = await _userManager.GetAuthenticatorKeyAsync(usuario);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(usuario);
                key = await _userManager.GetAuthenticatorKeyAsync(usuario);
            }

            if (string.IsNullOrEmpty(key)) return null;
            var email = usuario.Email ?? usuario.UserName ?? "usuario";
            var issuer = "ElectricERP";
            var authenticatorUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" + $"?secret={key}" + $"&issuer={Uri.EscapeDataString(issuer)}";
            return new TwoFactorSetupDto
            {
                SharedKey = key,
                AuthenticatorUri = authenticatorUri,
                QrCodeBase64 = string.Empty
            };
        }

        public async Task<bool> EnableTwoFactorAsync(string userId,string code)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;
            if (!usuario.Activo) return false;
            var validCode = await _userManager.VerifyTwoFactorTokenAsync(usuario,TokenOptions.DefaultAuthenticatorProvider,code);
            if (!validCode) return false;
            var result = await _userManager.SetTwoFactorEnabledAsync(usuario,true);
            return result.Succeeded;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes =RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private string HashRefreshToken(string refreshToken)
        {
            var bytes =SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(bytes);
        }

        private async Task<string> CreateRefreshTokenAsync(ApplicationUser usuario)
        {
            var refreshToken = GenerateRefreshToken();
            usuario.RefreshTokenHash =HashRefreshToken(refreshToken);
            usuario.RefreshTokenExpiryTime =DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(usuario);
            return refreshToken;
        }
    }

}