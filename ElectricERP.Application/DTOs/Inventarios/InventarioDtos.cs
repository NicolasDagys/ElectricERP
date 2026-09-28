using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ElectricERP.Application.DTOs.Inventarios
{
    public class ItemInventarioDto
    {
        public int IdProducto { get; set; }
        public int CantidadInventario { get; set; }
    }

    public class CrearInventarioDto
    {
        public int IdSeccion { get; set; }
    }

    public class ActualizarInventarioDto
    {
        public List<ItemInventarioDto> Productos { get; set; } = new();
    }

    public class DetalleInventarioItemDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public int CantidadInventario { get; set; }
        public int CantidadSistema { get; set; }
        public int Diferencia => CantidadInventario - CantidadSistema;
    }

    public class InventarioDto
    {
        public int IdInventario { get; set; }
        public int IdSeccion { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public string NombreAlmacen { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaCierre { get; set; }
        public List<DetalleInventarioItemDto> Detalles { get; set; } = new();
    }
}