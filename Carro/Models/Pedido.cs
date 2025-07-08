namespace Carro.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public List<DetallePedido> DetallePedido { get; set; }
        public double Total { get; set; }
        public DateTime FechaPedido { get; set; }
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
