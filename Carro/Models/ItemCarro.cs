namespace Carro.Models
{
    public class ItemCarro
    {
        public int ItemCarroId { get; set; }

        public Product Producto { get; set; }

        public int Cantidad { get; set; }

        public string CarroCompraId { get; set; }
    }
}
