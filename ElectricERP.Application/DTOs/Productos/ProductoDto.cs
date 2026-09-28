namespace ElectricERP.Application.DTOs.Productos
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
    }
}