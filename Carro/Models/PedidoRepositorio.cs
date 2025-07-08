using System.Security.Claims;

namespace Carro.Models
{
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private readonly AppDbContext _context;
        private readonly CarroCompra _carro;
        private IHttpContextAccessor _httpContext;


        public PedidoRepositorio(AppDbContext context, CarroCompra carro, IHttpContextAccessor httpContext)
        {
            _context = context;
            _carro = carro;
            _httpContext = httpContext;
        }

        public void CrearPedido(Pedido P)
        {
            using (var Transaccion = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(P);
                    _context.SaveChanges();

                    foreach (var item in _carro.ItemsCarro)
                    {
                        var detallePedido = new DetallePedido()
                        {
                            Cantidad = item.Cantidad,
                            ProductoId = item.Producto.ProductId,
                            PedidoId = P.PedidoId,
                            Precio = item.Producto.Precio
                        };
                        _context.Add(detallePedido);
                        var p = _context.tblProductos.Where(p => p.ProductId == item.Producto.ProductId).FirstOrDefault();
                        p.Stock -= item.Cantidad;
                        _context.Update(p);
                    }
                    _context.SaveChanges();
                    Transaccion.Commit();
                }
                catch (Exception)
                {

                    Transaccion.Rollback();
                }
                
            }
        }
    }
}

