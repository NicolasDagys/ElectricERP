using ElectricERP.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto model);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto model);
        Task<bool> LogoutAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto model);

        Task<AuthResponseDto?> VerifyTwoFactorAsync(VerifyTwoFactorDto model);

        Task<TwoFactorSetupDto?> SetupTwoFactorAsync(string userId);

        Task<bool> EnableTwoFactorAsync(string userId, string code);
    }
}
