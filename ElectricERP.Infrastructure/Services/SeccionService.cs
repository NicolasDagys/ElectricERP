using ElectricERP.Application.DTOs.Secciones;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class SeccionService : ISeccionService
    {
        private readonly ElectricERPDbContext _context;

        public SeccionService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<SeccionDto>> ObtenerTodosAsync()
        {
            return await _context.Set<ElectricERP.Infrastructure.Entities.Seccion>()
                .Include(s => s.IdAlmacenNavigation)
                .Select(s => new SeccionDto
                {
                    IdSeccion = s.IdSeccion,
                    Nombre = s.Nombre ?? string.Empty,
                    Descripcion = s.Descripcion ?? string.Empty,
                    IdAlmacen = s.IdAlmacen,
                    NombreAlmacen = s.IdAlmacenNavigation != null ? s.IdAlmacenNavigation.Nombre : string.Empty
                })
                .ToListAsync();
        }

        public async Task<SeccionDto?> ObtenerPorIdAsync(int id)
        {
            var s = await _context.Set<ElectricERP.Infrastructure.Entities.Seccion>()
                .Include(s => s.IdAlmacenNavigation)
                .FirstOrDefaultAsync(x => x.IdSeccion == id);

            if (s == null) return null;

            return new SeccionDto
            {
                IdSeccion = s.IdSeccion,
                Nombre = s.Nombre ?? string.Empty,
                Descripcion = s.Descripcion ?? string.Empty,
                IdAlmacen = s.IdAlmacen,
                NombreAlmacen = s.IdAlmacenNavigation != null ? s.IdAlmacenNavigation.Nombre : string.Empty
            };
        }

        public async Task<SeccionDto> CrearAsync(CrearSeccionDto dto)
        {
            var seccion = new ElectricERP.Infrastructure.Entities.Seccion
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                IdAlmacen = dto.IdAlmacen
            };

            _context.Set<ElectricERP.Infrastructure.Entities.Seccion>().Add(seccion);
            await _context.SaveChangesAsync();

            await _context.Entry(seccion).Reference(x => x.IdAlmacenNavigation).LoadAsync();

            return new SeccionDto
            {
                IdSeccion = seccion.IdSeccion,
                Nombre = seccion.Nombre ?? string.Empty,
                Descripcion = seccion.Descripcion ?? string.Empty,
                IdAlmacen = seccion.IdAlmacen,
                NombreAlmacen = seccion.IdAlmacenNavigation != null ? seccion.IdAlmacenNavigation.Nombre : string.Empty
            };
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var seccion = await _context.Set<ElectricERP.Infrastructure.Entities.Seccion>().FindAsync(id);
            if (seccion == null) return (false, "No encontrado");

            _context.Set<ElectricERP.Infrastructure.Entities.Seccion>().Remove(seccion);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }
    }
}