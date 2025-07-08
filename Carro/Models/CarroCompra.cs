using Microsoft.EntityFrameworkCore;

namespace Carro.Models
{
    public class CarroCompra
    {
        private readonly AppDbContext _context;

        public CarroCompra(AppDbContext context)
        {
            _context = context;
        }
        public string CarroCompraId { get; set; }
        public List<ItemCarro> ItemsCarro { get; set; }


        public static CarroCompra GetCarro(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDbContext>();
            string CarroId = session.GetString("CarroId") ?? Guid.NewGuid().ToString();
            session.SetString("CarroId", CarroId);
            return new CarroCompra(context) { CarroCompraId = CarroId };
        }


        public void AddItem(Product P)
        {
            var item = _context.ItemsCarro
                .Where(i => i.Producto.ProductId == P.ProductId && i.CarroCompraId == this.CarroCompraId)
                .FirstOrDefault();

            if (item == null)
            {
                item = new ItemCarro
                {
                    CarroCompraId = this.CarroCompraId,
                    Producto = P,
                    Cantidad = 1
                };
                _context.Add(item);
            }
            else
            {
                item.Cantidad++;
            }
            _context.SaveChanges();
        }

        public void DelItem(Product P)
        {
            var item = _context.ItemsCarro
                .Where(i => i.Producto.ProductId == P.ProductId && i.CarroCompraId == this.CarroCompraId)
                .FirstOrDefault();

            if (item != null)
            {
                if (item.Cantidad > 1)
                {
                    item.Cantidad--;
                }
                else
                {
                    _context.Remove(item);
                }
            }
            _context.SaveChanges();
        }

        public List<ItemCarro> GetItemsCarro()
        {
            return _context.ItemsCarro
                .Where(i => i.CarroCompraId == this.CarroCompraId)
                .Include(i => i.Producto)
                .ToList();
        }

        public void VaciarCarro()
        {
            var items = _context.ItemsCarro
                .Where(i => i.CarroCompraId == this.CarroCompraId)
                .ToList();
            _context.ItemsCarro.RemoveRange(items);
            _context.SaveChanges();
        }

        public double GetTotal()
        {
            return _context.ItemsCarro.Where(i => i.CarroCompraId == this.CarroCompraId)
                .Select(i => i.Producto.Precio * i.Cantidad).Sum();
        }
    }
}
