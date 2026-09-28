namespace ElectricERP.Application.DTOs.Productos
{
    public class ActualizarProductoDto
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        // CodigoSku no se edita: es el identificador único del producto una vez creado.
    }
}