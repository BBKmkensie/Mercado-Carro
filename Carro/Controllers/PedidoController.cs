using Carro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace Carro.Controllers
{
    public class PedidoController : Controller
    {

        private readonly AppDbContext _context;
        private readonly CarroCompra _carro;
        private readonly IPedidoRepositorio _pedido;
        public PedidoController(AppDbContext context, CarroCompra carro, IPedidoRepositorio pedido)
        {
            _context = context;
            _carro = carro;
            _pedido = pedido;
        }

        public IActionResult ConfirmarPedido()
        {
            var items = _carro.GetItemsCarro();
            _carro.ItemsCarro = items;

            CarroViewModel Cvm = new CarroViewModel
            {
                Carro = _carro,
                Total = _carro.GetTotal()
            };
            return View(Cvm);
        }

        [HttpGet]
        public IActionResult ConfirmarPedidoPost()
        {
            var usuario = _context.tblUsuario.Where(x => x.Username == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault();

            var items = _carro.GetItemsCarro();
            _carro.ItemsCarro = items;
            CarroViewModel Cvm = new CarroViewModel
            {
                Carro = _carro,
                Total = _carro.GetTotal()
            };

            if (_carro.ItemsCarro.Count==0)
            {
                ModelState.AddModelError("", "No tienes productos agregados en tu carro de compras. Añade algunos primero.");
                return View(Cvm);
            }
            else
            {
                var P = new Pedido()
                {
                    FechaPedido = DateTime.Now,
                    Total = _carro.GetTotal(),
                    UsuarioId = usuario.UsuarioId
                };
                _pedido.CrearPedido(P);
                _carro.VaciarCarro();
                return RedirectToAction(nameof(PedidoCompletado));

            }

        }

        public IActionResult PedidoCompletado()
        {
            ViewBag.PedidoCompletadoMensaje = "Gracias por Comprar con nosotros. Pronto recibiras un Email Confirmando tu Pedido.";
            return View();
        }
    }
}
