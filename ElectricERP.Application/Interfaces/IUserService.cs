using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Users;

namespace ElectricERP.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> ObtenerTodosAsync();
        Task<UserDto?> ObtenerPorIdAsync(string id);
        Task<UserDto> CrearAsync(CreateUserDto dto);
        Task<bool> ActualizarAsync(string id, UpdateUserDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(string id);
        Task<bool> CambiarEstadoAsync(string id, bool activo);
        Task<bool> ActualizarPerfilAsync(string email, UpdateProfileDto dto);
        Task<UserDto?> ObtenerPerfilAsync(string email);
        Task<bool> ResetearPasswordAsync(string id, string nuevaPassword);
    }
}