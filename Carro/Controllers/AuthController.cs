using Carro.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Carro.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult LoginIn()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecuperarPass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarPass(RecuperarPassViewModel Rvm)
        {
            if (ModelState.IsValid)
            {
                var u = _context.tblUsuario.Where(u => u.Email == Rvm.Correo).FirstOrDefault();
                if(u!= null)
                {
                    CreatePasswordHash(Rvm.Password, out byte[] hash, out byte[] salt);
                    u.PasswordHash = hash;
                    u.PasswordSalt = salt;
                    _context.Update(u);
                    _context.SaveChanges();
                    return RedirectToAction("LoginIn");
                }
            }
            return View(Rvm);
        }

        [HttpPost]
        public IActionResult Registro(RegistroViewModel Rvm)
        {
            if (ModelState.IsValid)
            {
                Usuario U = new Usuario();
                U.Email = Rvm.Correo;
                U.Username = Rvm.Username;
                U.Name = Rvm.Nombre;
                U.Rol = "Cliente";

                CreatePasswordHash(Rvm.Password, out byte[] hash, out byte[] salt);

                U.PasswordHash = hash;
                U.PasswordSalt = salt;

                _context.tblUsuario.Add(U);
                _context.SaveChanges();

                return RedirectToAction("LoginIn");
            }
            else
            {
                ModelState.AddModelError("", "Error al Registrarse!");
                return View(Rvm);
            }

            
        }


        [HttpGet]
        public IActionResult HistorialPedidos()
        {
            if (User.Identity.IsAuthenticated)
            {
                var usuario = _context.tblUsuario.Where(x => x.Username == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault();
                return View(_context.Pedidos.Where(p => p.UsuarioId == usuario.UsuarioId).ToList());
            }
            else
            {
                return RedirectToAction("LoginIn", "Auth");
            }
            
        }

        [HttpGet]
        public IActionResult DetallePedido(int PedidoId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var usuario = _context.tblUsuario.Where(x => x.Username == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault();
                var detallepedido = _context.DetallePedido
                    .Where(d => d.PedidoId == PedidoId)
                    .Include(d=>d.Pedido)
                    .Include(d=>d.Producto)
                    .ToList();
                return View(detallepedido);
            }
            else
            {
                return RedirectToAction("LoginIn", "Auth");
            }

        }


        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginViewModel Lvm)
        {
            var usuarios = _context.tblUsuario.ToList();
            if(usuarios .Count == 0)
            {
                var U = new Usuario();
                U.Name = "Administrador";
                U.Email = "admi@inacap.cl";
                U.Username = "Admin";
                U.Rol = "SuperAdministrador";

                CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.tblUsuario.Add(U);
                _context.SaveChanges();


            }

            var us = _context.tblUsuario.Where(u => u.Username.Equals(Lvm.Username)).FirstOrDefault();
            if(us != null)
            {
                //Usuario Encontrado
                if (VerificarPass(Lvm.Password, us.PasswordHash, us.PasswordSalt))
                {
                    //Usuario y contraseña Correctos!
                    var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, us.Username),
                        new Claim(ClaimTypes.NameIdentifier, Lvm.Username),
                        new Claim(ClaimTypes.Role, us.Rol),
                    };

                    //Carnet, Licencia
                    var identity = new ClaimsIdentity(Claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var pricipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pricipal,
                        new AuthenticationProperties { IsPersistent = true}
                        );

                    return RedirectToAction("Clientes", "Home");

                }
                else
                {
                    //Usuario correcto pero contraseña mala
                    ModelState.AddModelError("", "Usuario no Encontrado!");
                    return View(Lvm);
                }


            }
            else
            {
                //Uusario No Existe
                ModelState.AddModelError("", "Usuario no Encontrado!");
                return View(Lvm);
            }


            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                string username = "";
                string rol = "";
                foreach (var claim in User.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                    {
                        username = claim.Value;
                        rol = claim.Value;
                    }
                }


                var u = _context.tblUsuario.Where(x => x.Username.Equals(username)).FirstOrDefault();
                ProfileViewModel Pvm = new ProfileViewModel()
                {
                    UsersId = u.UsuarioId,
                    Username = u.Username,
                    Rol = u.Rol,
                    Email = u.Email,
                    Name = u.Name
                };

                return View(Pvm);
            }
            return RedirectToAction(nameof(LoginIn));

        }

        [HttpGet]
        public IActionResult LoginGoogle()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/Home/Index" }, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Solo cerrar sesión en Google si el usuario inició sesión con Google
            var isGoogleUser = User.Claims.Any(c => c.Issuer == "https://accounts.google.com" );

            if (isGoogleUser)
            {
                return Redirect("https://accounts.google.com/Logout");
            }

            return RedirectToAction("LoginIn", "Auth");
        }





        public IActionResult AccessDenied()
        {
            return View();
        }




        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwodSalt)
        {
            //123456 Administrador
            using (var hmac = new HMACSHA512())
            {
                passwodSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private bool VerificarPass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var pass=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }

        }
    }
}
