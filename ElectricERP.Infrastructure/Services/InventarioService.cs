using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Inventarios;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly ElectricERPDbContext _context;

        public InventarioService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventarioDto>> ObtenerTodosAsync()
        {
            var lista = await BaseQuery().OrderByDescending(i => i.FechaAlta).ToListAsync();
            var resultado = new List<InventarioDto>();
            foreach (var inv in lista)
                resultado.Add(await MapToDtoAsync(inv));
            return resultado;
        }

        public async Task<InventarioDto?> ObtenerPorIdAsync(int id)
        {
            var inv = await BaseQuery().FirstOrDefaultAsync(i => i.IdInventario == id);
            return inv == null ? null : await MapToDtoAsync(inv);
        }

        public async Task<(bool Exito, string? Mensaje, InventarioDto? Creado)> CrearAsync(CrearInventarioDto dto)
        {
            var seccion = await _context.Set<Seccion>().FirstOrDefaultAsync(s => s.IdSeccion == dto.IdSeccion);
            if (seccion == null) return (false, "La sección seleccionada no existe.", null);

            var inventario = new Inventario
            {
                IdSeccion = dto.IdSeccion,
                Estado = "ABIERTO",
                FechaAlta = DateTime.Now
            };

            try
            {
                _context.Set<Inventario>().Add(inventario);
                await _context.SaveChangesAsync();

                var creado = await BaseQuery().FirstAsync(i => i.IdInventario == inventario.IdInventario);
                return (true, null, await MapToDtoAsync(creado));
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarDetalleAsync(int id, ActualizarInventarioDto dto)
        {
            var inventario = await _context.Set<Inventario>()
                .Include(i => i.DetalleInventarios)
                .FirstOrDefaultAsync(i => i.IdInventario == id);

            if (inventario == null) return (false, "No encontrado");

            if (inventario.Estado != "ABIERTO")
                return (false, "No se puede modificar el detalle de un inventario ya finalizado.");

            if (dto.Productos == null || !dto.Productos.Any())
                return (false, "Tenés que cargar al menos un producto contado.");

            foreach (var item in dto.Productos)
            {
                if (item.CantidadInventario < 0)
                    return (false, "Las cantidades no pueden ser negativas.");

                var existente = inventario.DetalleInventarios.FirstOrDefault(d => d.IdProducto == item.IdProducto);
                if (existente != null)
                {
                    existente.CantidadInventario = item.CantidadInventario;
                }
                else
                {
                    inventario.DetalleInventarios.Add(new DetalleInventario
                    {
                        IdInventario = id,
                        IdProducto = item.IdProducto,
                        CantidadInventario = item.CantidadInventario
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        public async Task<(bool Exito, string? Mensaje)> CerrarAsync(int id)
        {
            var inventario = await _context.Set<Inventario>()
                .Include(i => i.DetalleInventarios)
                .FirstOrDefaultAsync(i => i.IdInventario == id);

            if (inventario == null) return (false, "No encontrado");

            if (inventario.Estado != "ABIERTO")
                return (false, "Este inventario ya está finalizado.");

            if (!inventario.DetalleInventarios.Any())
                return (false, "No se puede cerrar un inventario sin ningún producto contado.");

            inventario.Estado = "FINALIZADO";
            inventario.FechaCierre = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        private IQueryable<Inventario> BaseQuery()
        {
            return _context.Set<Inventario>()
                .Include(i => i.IdSeccionNavigation)
                    .ThenInclude(s => s.IdAlmacenNavigation)
                .Include(i => i.DetalleInventarios)
                    .ThenInclude(d => d.IdProductoNavigation);
        }

        private async Task<InventarioDto> MapToDtoAsync(Inventario inv)
        {
            // Comparamos contra el Stock teórico de esa sección para calcular la diferencia
            var stockPorProducto = await _context.Set<Stock>()
                .Where(s => s.IdSeccion == inv.IdSeccion)
                .ToDictionaryAsync(s => s.IdProducto, s => s.CantidadActual);

            return new InventarioDto
            {
                IdInventario = inv.IdInventario,
                IdSeccion = inv.IdSeccion,
                NombreSeccion = inv.IdSeccionNavigation?.Nombre ?? string.Empty,
                NombreAlmacen = inv.IdSeccionNavigation?.IdAlmacenNavigation?.Nombre ?? string.Empty,
                Estado = inv.Estado,
                FechaAlta = inv.FechaAlta,
                FechaCierre = inv.FechaCierre,
                Detalles = inv.DetalleInventarios.Select(d => new DetalleInventarioItemDto
                {
                    IdProducto = d.IdProducto,
                    NombreProducto = d.IdProductoNavigation?.Nombre ?? string.Empty,
                    CodigoSku = d.IdProductoNavigation?.CodigoSku ?? string.Empty,
                    CantidadInventario = d.CantidadInventario,
                    CantidadSistema = stockPorProducto.TryGetValue(d.IdProducto, out var cant) ? cant : 0
                }).ToList()
            };
        }

        private static string TraducirErrorSql(DbUpdateException ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;

            if (mensaje.Contains("CK_DetalleInventario_Cantidad"))
                return "Las cantidades contadas no pueden ser negativas.";
            if (mensaje.Contains("CK_Inventario_Fechas"))
                return "La fecha de cierre no puede ser anterior a la de alta.";

            return $"No se pudo guardar el inventario. Detalle: {mensaje}";
        }
    }
}