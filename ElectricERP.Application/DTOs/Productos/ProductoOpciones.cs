namespace ElectricERP.Application.DTOs.Productos
{
    public static class ProductoOpciones
    {
        public static readonly string[] Categorias = new[]
        {
            "Resistencias", "Capacitores", "Inductores", "Diodos", "Transistores",
            "Circuitos", "Integrados", "Sensores", "Conectores", "Cables",
            "Fuentes de Alimentacion", "Reles", "Fusibles", "Herramientas", "Accesorios"
        };

        public static readonly string[] UnidadesMedida = new[]
        {
            "m", "cm", "kg", "Consumo Electrico"
        };
    }
}