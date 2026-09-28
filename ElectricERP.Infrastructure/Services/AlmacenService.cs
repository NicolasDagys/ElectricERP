using ElectricERP.Application.DTOs.Almacenes;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class AlmacenService : IAlmacenService
    {
        private readonly ElectricERPDbContext _context;

        public AlmacenService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<AlmacenDto>> ObtenerTodosAsync()
        {
            return await _context.Almacenes
                .Select(a => new AlmacenDto
                {
                    IdAlmacen = a.IdAlmacen,
                    Nombre = a.Nombre,
                    Direccion = a.Direccion ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<AlmacenDto?> ObtenerPorIdAsync(int id)
        {
            var a = await _context.Almacenes.FindAsync(id);
            if (a == null) return null;

            return new AlmacenDto
            {
                IdAlmacen = a.IdAlmacen,
                Nombre = a.Nombre,
                Direccion = a.Direccion ?? string.Empty
            };
        }

        public async Task<AlmacenDto> CrearAsync(CrearAlmacenDto dto)
        {
            var almacen = new ElectricERP.Infrastructure.Entities.Almacen
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion
            };

            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            return new AlmacenDto
            {
                IdAlmacen = almacen.IdAlmacen,
                Nombre = almacen.Nombre,
                Direccion = almacen.Direccion ?? string.Empty
            };
        }

        public async Task<bool> ActualizarAsync(int id, CrearAlmacenDto dto)
        {
            var almacen = await _context.Almacenes.FindAsync(id);
            if (almacen == null) return false;

            almacen.Nombre = dto.Nombre;
            almacen.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);
            if (almacen == null) return (false, "No encontrado");

            bool tieneSecciones = await _context.Set<ElectricERP.Infrastructure.Entities.Seccion>()
                                              .AnyAsync(s => s.IdAlmacen == id);

            if (tieneSecciones)
            {
                return (false, "No se puede eliminar el almacén porque contiene secciones asociadas.");
            }

            _context.Almacenes.Remove(almacen);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}