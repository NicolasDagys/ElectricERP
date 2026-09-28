using ElectricERP.Application.DTOs.Productos;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ElectricERPDbContext _context;

        public ProductoService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDto>> ObtenerTodosAsync()
        {
            return await _context.Set<Producto>()
                .Include(p => p.IdProveedorNavigation)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
        {
            var producto = await _context.Set<Producto>()
                .Include(p => p.IdProveedorNavigation)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            return producto == null ? null : MapToDto(producto);
        }

        public async Task<List<ProductoDto>> BuscarAsync(string texto)
        {
            texto = texto?.Trim() ?? string.Empty;

            var query = _context.Set<Producto>().Include(p => p.IdProveedorNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(texto))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(texto) ||
                    p.CodigoSku.Contains(texto) ||
                    (p.Descripcion != null && p.Descripcion.Contains(texto)));
            }

            return await query.Select(p => MapToDto(p)).ToListAsync();
        }

        public async Task<(bool Exito, string? Mensaje, ProductoDto? Creado)> CrearAsync(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                IdProveedor = dto.IdProveedor,
                Nombre = dto.Nombre,
                CodigoSku = dto.CodigoSku.ToUpperInvariant(),
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                UnidadMedida = dto.UnidadMedida
            };

            try
            {
                _context.Set<Producto>().Add(producto);
                await _context.SaveChangesAsync();

                await _context.Entry(producto).Reference(p => p.IdProveedorNavigation).LoadAsync();

                return (true, null, MapToDto(producto));
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarProductoDto dto)
        {
            var producto = await _context.Set<Producto>().FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null) return (false, "No encontrado");

            producto.IdProveedor = dto.IdProveedor;
            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Categoria = dto.Categoria;
            producto.UnidadMedida = dto.UnidadMedida;

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

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var producto = await _context.Set<Producto>().FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null) return (false, "No encontrado");

            try
            {
                _context.Set<Producto>().Remove(producto);
                await _context.SaveChangesAsync();
                return (true, "Eliminado");
            }
            catch (DbUpdateException)
            {
                return (false, "No se puede eliminar: el producto tiene registros asociados (stock, movimientos, etiquetas, inventarios o transferencias).");
            }
        }

        private static string TraducirErrorSql(DbUpdateException ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;

            if (mensaje.Contains("UQ_Producto_SKU"))
                return "Ya existe un producto con ese código (SKU).";
            if (mensaje.Contains("CodigoSKU") || mensaje.Contains("CK__Producto"))
                return "El código (SKU) no tiene el formato correcto: solo mayúsculas y números, mínimo 11 caracteres, con al menos una letra y un número.";
            if (mensaje.Contains("FK_Producto_Proveedor"))
                return "El proveedor seleccionado no existe.";
            if (mensaje.Contains("Categoria"))
                return "La categoría seleccionada no es válida.";
            if (mensaje.Contains("UnidadMedida"))
                return "La unidad de medida seleccionada no es válida.";

            return $"No se pudo guardar el producto. Detalle: {mensaje}";
        }

        private static ProductoDto MapToDto(Producto p) => new()
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            CodigoSku = p.CodigoSku,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            UnidadMedida = p.UnidadMedida,
            IdProveedor = p.IdProveedor,
            NombreProveedor = p.IdProveedorNavigation?.Nombre ?? string.Empty
        };
    }
}