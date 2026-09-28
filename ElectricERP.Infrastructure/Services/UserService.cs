using ElectricERP.Application.DTOs.Users;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricERP.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDto>> ObtenerTodosAsync()
        {
            var usuarios = await _userManager.Users
                .ToListAsync();

            var resultado = new List<UserDto>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                resultado.Add(new UserDto
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName ?? string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? string.Empty,
                    Activo = usuario.Activo
                });
            }

            return resultado;
        }

        public async Task<UserDto?> ObtenerPorIdAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return null;

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UserDto
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Rol = roles.FirstOrDefault() ?? string.Empty,
                Activo = usuario.Activo
            };
        }

        public async Task<UserDto> CrearAsync(CreateUserDto dto)
        {
            // Verificar que el rol solicitado exista
            if (!await _roleManager.RoleExistsAsync(dto.Rol))
            {
                throw new Exception(
                    $"El rol '{dto.Rol}' no existe.");
            }

            var usuarioExistente =
                await _userManager.FindByEmailAsync(dto.Email);

            if (usuarioExistente != null)
            {
                throw new Exception(
                    "Ya existe un usuario con ese email.");
            }

            var usuario = new ApplicationUser
            {
                UserName = dto.NombreCompleto,
                Email = dto.Email,
                EmailConfirmed = true,
                Activo = dto.Activo
            };

            // Identity genera PasswordHash, SecurityStamp,
            // ConcurrencyStamp, etc.
            var createResult = await _userManager.CreateAsync(
                usuario,
                dto.Password);

            if (!createResult.Succeeded)
            {
                var errores = string.Join(
                    ", ",
                    createResult.Errors.Select(e => e.Description));

                throw new Exception(
                    $"No se pudo crear el usuario: {errores}");
            }

            // Asignar rol mediante Identity
            var roleResult = await _userManager.AddToRoleAsync(
                usuario,
                dto.Rol);

            if (!roleResult.Succeeded)
            {
                var errores = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                // Si falla la asignación del rol,
                // eliminamos el usuario recién creado.
                await _userManager.DeleteAsync(usuario);

                throw new Exception(
                    $"No se pudo asignar el rol: {errores}");
            }

            return new UserDto
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Rol = dto.Rol,
                Activo = usuario.Activo
            };
        }

        public async Task<bool> ActualizarAsync(
            string id,
            UpdateUserDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return false;

            // Verificar que el nuevo rol exista
            if (!await _roleManager.RoleExistsAsync(dto.Rol))
                return false;

            usuario.UserName = dto.NombreCompleto;

            var updateResult =
                await _userManager.UpdateAsync(usuario);

            if (!updateResult.Succeeded)
                return false;

            // Obtener roles actuales
            var rolesActuales =
                await _userManager.GetRolesAsync(usuario);

            // Si tiene roles, quitarlos
            if (rolesActuales.Any())
            {
                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        usuario,
                        rolesActuales);

                if (!removeResult.Succeeded)
                    return false;
            }

            // Asignar el nuevo rol
            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    usuario,
                    dto.Rol);

            return addRoleResult.Succeeded;
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(
            string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return (false, "No encontrado");

            var result =
                await _userManager.DeleteAsync(usuario);

            if (!result.Succeeded)
            {
                var errores = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                return (false, errores);
            }

            return (true, string.Empty);
        }

        public async Task<bool> CambiarEstadoAsync(
            string id,
            bool activo)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return false;

            usuario.Activo = activo;

            var result =
                await _userManager.UpdateAsync(usuario);

            return result.Succeeded;
        }

        public async Task<UserDto?> ObtenerPerfilAsync(
            string email)
        {
            var usuario =
                await _userManager.FindByEmailAsync(email);

            if (usuario == null)
                return null;

            var roles =
                await _userManager.GetRolesAsync(usuario);

            return new UserDto
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Rol = roles.FirstOrDefault() ?? string.Empty,
                Activo = usuario.Activo
            };
        }

        public async Task<bool> ActualizarPerfilAsync(
            string email,
            UpdateProfileDto dto)
        {
            var usuario =
                await _userManager.FindByEmailAsync(email);

            if (usuario == null)
                return false;

            usuario.UserName = dto.NombreCompleto;

            var result =
                await _userManager.UpdateAsync(usuario);

            return result.Succeeded;
        }

        public async Task<bool> ResetearPasswordAsync(
            string id,
            string nuevaPassword)
        {
            var usuario =
                await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return false;

            // Generar token de Identity para resetear password
            var resetToken =
                await _userManager.GeneratePasswordResetTokenAsync(
                    usuario);

            var result =
                await _userManager.ResetPasswordAsync(
                    usuario,
                    resetToken,
                    nuevaPassword);

            return result.Succeeded;
        }
    }
}