using ElectricERP.Application.DTOs.Auth;
using ElectricERP.Application.Interfaces;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElectricERP.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IQrCodeService _qrCodeService;
        public AuthController(IApiClient apiClient,IQrCodeService qrCodeService)
        {
            _apiClient = apiClient;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var resultado = await _apiClient.LoginAsync(dto);
            if (resultado == null)
            {
                ViewBag.Error = "Credenciales incorrectas o servidor no disponible.";
                return View(dto);
            }
            if (resultado.RequiresTwoFactor)
            {
                if (string.IsNullOrEmpty(resultado.TwoFactorToken))
                {
                    ViewBag.Error = "No se pudo iniciar el proceso de autenticación 2FA.";
                    return View(dto);
                }

                return RedirectToAction("VerifyTwoFactor", new { twoFactorToken = resultado.TwoFactorToken, email = dto.Email });
            }
            if (string.IsNullOrEmpty(resultado.Token))
            {
                ViewBag.Error = "Credenciales incorrectas o servidor no disponible.";
                return View(dto);
            }
            await SignInUserAsync( dto.Email, resultado.Token);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(string email, string token)
        {
            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.Name, email ?? string.Empty),
                new Claim("JWToken", token)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyTwoFactor(string twoFactorToken,string email)
        {
            if (string.IsNullOrEmpty(twoFactorToken))
            {
                return RedirectToAction("Login");
            }
            ViewBag.Email = email;
            var model = new VerifyTwoFactorDto
            {
                TwoFactorToken = twoFactorToken
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var resultado = await _apiClient.VerifyTwoFactorAsync(dto);
            if (resultado == null || string.IsNullOrEmpty(resultado.Token))
            {
                ViewBag.Error = "El código de autenticación es incorrecto o expiró.";
                return View(dto);
            }
            await SignInUserAsync(resultado.Email, resultado.Token);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CambiarPassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var token = User.FindFirst("JWToken")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "No se encontró la sesión activa. Por favor, vuelva a iniciar sesión.");
                return View(dto);
            }
            var exito = await _apiClient.CambiarPasswordAsync(dto, token);
            if (exito)
            {
                TempData["MensajeExito"] = "Contraseña actualizada correctamente.";
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "No se pudo actualizar la contraseña. Verifique los datos.");
            return View(dto);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfigurarTwoFactor()
        {
            var token = User.FindFirst("JWToken")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var setup = await _apiClient.SetupTwoFactorAsync(token);

            if (setup == null)
            {
                ViewBag.Error = "No se pudo obtener la configuración de autenticación de dos factores.";
                return View(new TwoFactorSetupDto());
            }

            setup.QrCodeBase64 = _qrCodeService.GenerateBase64(setup.AuthenticatorUri);
            return View(setup);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EnableTwoFactor(string code)
        {
            var token = User.FindFirst("JWToken")?.Value;
            if (string.IsNullOrEmpty(token)) return RedirectToAction(nameof(Login));
            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["Error"] ="Debe ingresar el código de autenticación.";
                return RedirectToAction(nameof(ConfigurarTwoFactor));
            }
            var success = await _apiClient.EnableTwoFactorAsync(code, token);
            if (!success)
            {
                TempData["Error"] ="El código de autenticación es incorrecto.";
                return RedirectToAction(nameof(ConfigurarTwoFactor));
            }
            TempData["MensajeExito"] ="La autenticación de dos factores fue activada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
