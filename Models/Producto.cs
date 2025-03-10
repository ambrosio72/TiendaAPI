namespace TiendaAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateOnly FechaSubida { get; set; }
        public bool Nuevo { get; set; }
        public string Foto { get; set; }
        public decimal Descuento { get; set; }
        public string Vendedor { get; set; }
    }
}
