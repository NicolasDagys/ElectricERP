using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Stock;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly ElectricERPDbContext _context;

        public StockService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockDto>> ObtenerTodosAsync()
        {
            return await _context.Set<Stock>()
                .Include(s => s.IdProductoNavigation)
                .Include(s => s.IdSeccionNavigation)
                    .ThenInclude(sec => sec.IdAlmacenNavigation)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<StockDto?> ObtenerPorIdAsync(int id)
        {
            var stock = await _context.Set<Stock>()
                .Include(s => s.IdProductoNavigation)
                .Include(s => s.IdSeccionNavigation)
                    .ThenInclude(sec => sec.IdAlmacenNavigation)
                .FirstOrDefaultAsync(s => s.IdStock == id);

            return stock == null ? null : MapToDto(stock);
        }

        public async Task<List<StockDto>> ObtenerAlertasAsync()
        {
            return await _context.Set<Stock>()
                .Include(s => s.IdProductoNavigation)
                .Include(s => s.IdSeccionNavigation)
                    .ThenInclude(sec => sec.IdAlmacenNavigation)
                .Where(s => s.CantidadActual <= s.Minimo || s.CantidadActual >= s.Maximo)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<(bool Exito, string? Mensaje, StockDto? Creado)> CrearAsync(CrearStockDto dto)
        {
            if (dto.Maximo <= dto.Minimo)
                return (false, "El máximo debe ser mayor al mínimo.", null);

            var yaExiste = await _context.Set<Stock>()
                .AnyAsync(s => s.IdProducto == dto.IdProducto && s.IdSeccion == dto.IdSeccion);

            if (yaExiste)
                return (false, "Ya existe un registro de stock para ese producto en esa sección.", null);

            var stock = new Stock
            {
                IdProducto = dto.IdProducto,
                IdSeccion = dto.IdSeccion,
                CantidadActual = dto.CantidadActual,
                Minimo = dto.Minimo,
                Maximo = dto.Maximo,
                UltimaActualizacion = DateTime.Now
            };

            try
            {
                _context.Set<Stock>().Add(stock);
                await _context.SaveChangesAsync();

                await _context.Entry(stock).Reference(s => s.IdProductoNavigation).LoadAsync();
                await _context.Entry(stock).Reference(s => s.IdSeccionNavigation).LoadAsync();
                await _context.Entry(stock.IdSeccionNavigation).Reference(sec => sec.IdAlmacenNavigation).LoadAsync();

                return (true, null, MapToDto(stock));
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo crear el stock. Verificá los datos ingresados.", null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> RegistrarCantidadAsync(int id, RegistrarStockDto dto)
        {
            var stock = await _context.Set<Stock>().FirstOrDefaultAsync(s => s.IdStock == id);
            if (stock == null) return (false, "No encontrado");

            if (dto.CantidadActual < 0)
                return (false, "La cantidad no puede ser negativa.");

            stock.CantidadActual = dto.CantidadActual;
            stock.UltimaActualizacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo registrar el stock. Verificá que la cantidad no supere el límite permitido.");
            }
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarLimitesAsync(int id, ActualizarLimitesDto dto)
        {
            var stock = await _context.Set<Stock>().FirstOrDefaultAsync(s => s.IdStock == id);
            if (stock == null) return (false, "No encontrado");

            if (dto.Maximo <= dto.Minimo)
                return (false, "El máximo debe ser mayor al mínimo.");

            stock.Minimo = dto.Minimo;
            stock.Maximo = dto.Maximo;
            stock.UltimaActualizacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudieron actualizar los límites.");
            }
        }

        private static StockDto MapToDto(Stock s)
        {
            var estado = "Normal";
            if (s.CantidadActual <= s.Minimo) estado = "Minimo";
            else if (s.CantidadActual >= s.Maximo) estado = "Maximo";

            return new StockDto
            {
                IdStock = s.IdStock,
                IdProducto = s.IdProducto,
                NombreProducto = s.IdProductoNavigation?.Nombre ?? string.Empty,
                CodigoSku = s.IdProductoNavigation?.CodigoSku ?? string.Empty,
                IdSeccion = s.IdSeccion,
                NombreSeccion = s.IdSeccionNavigation?.Nombre ?? string.Empty,
                NombreAlmacen = s.IdSeccionNavigation?.IdAlmacenNavigation?.Nombre ?? string.Empty,
                CantidadActual = s.CantidadActual,
                Minimo = s.Minimo,
                Maximo = s.Maximo,
                UltimaActualizacion = s.UltimaActualizacion,
                EstadoAlerta = estado
            };
        }
    }
}