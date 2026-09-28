using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Suppliers;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ElectricERPDbContext _context;

        public SupplierService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierDto>> ObtenerTodosAsync()
        {
            return await _context.Proveedores
                .Select(p => new SupplierDto
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    Rut = p.Rut ?? string.Empty,
                    Telefono = p.Telefono ?? string.Empty,
                    Email = p.Email ?? string.Empty,
                    Direccion = p.Direccion ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<SupplierDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _context.Proveedores.FindAsync(id);
            if (p == null) return null;

            return new SupplierDto
            {
                IdProveedor = p.IdProveedor,
                Nombre = p.Nombre,
                Rut = p.Rut ?? string.Empty,
                Telefono = p.Telefono ?? string.Empty,
                Email = p.Email ?? string.Empty,
                Direccion = p.Direccion ?? string.Empty
            };
        }

        public async Task<SupplierDto> CrearAsync(CrearSupplierDto dto)
        {
            var proveedor = new ElectricERP.Infrastructure.Entities.Proveedor
            {
                Nombre = dto.Nombre,
                Rut = dto.Rut,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return new SupplierDto
            {
                IdProveedor = proveedor.IdProveedor,
                Nombre = proveedor.Nombre,
                Rut = proveedor.Rut ?? string.Empty,
                Telefono = proveedor.Telefono ?? string.Empty,
                Email = proveedor.Email ?? string.Empty,
                Direccion = proveedor.Direccion ?? string.Empty
            };
        }

        public async Task<bool> ActualizarAsync(int id, CrearSupplierDto dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return false;

            proveedor.Nombre = dto.Nombre;
            proveedor.Rut = dto.Rut;
            proveedor.Telefono = dto.Telefono;
            proveedor.Email = dto.Email;
            proveedor.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return false;

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}