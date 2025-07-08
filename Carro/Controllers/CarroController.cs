using Carro.Models;

using Microsoft.AspNetCore.Mvc;



namespace Carro.Controllers
{
    public class CarroController : Controller
    {
        private readonly CarroCompra _carroCompra;
        private readonly AppDbContext _context;

        public CarroController(CarroCompra carroCompra, AppDbContext context )
        {
            _carroCompra = carroCompra;
            _context = context;
        }
        public IActionResult Index()
        {
            var items = _carroCompra.GetItemsCarro();
            _carroCompra.ItemsCarro = items;

            CarroViewModel Cvm = new CarroViewModel
            {
                Carro = _carroCompra,
                Total = _carroCompra.GetTotal()
            };
            return View(Cvm);
        }

        public RedirectToActionResult Add(int id)
        {
            var P = _context.tblProductos.Where(x => x.ProductId == id).FirstOrDefault();
            if (P != null)
            {
                if(P.Stock > 0)
                {
                    _carroCompra.AddItem(P);
                }                
            }
            return RedirectToAction(nameof(Index));
        }

        public RedirectToActionResult Del(int id)
        {
            var P = _context.tblProductos.Where(x => x.ProductId == id).FirstOrDefault();
            if (P != null)
            {
                _carroCompra.DelItem(P);
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
