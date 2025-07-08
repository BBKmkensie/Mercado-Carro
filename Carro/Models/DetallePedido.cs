namespace Carro.Models
{
    public class DetallePedido
    {
        public int DetallePedidoId { get; set; }

        public int PedidoId { get; set; }

        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        public double Precio { get; set; }

        public Product Producto { get; set; }

        public Pedido Pedido { get; set; }
    }
}
