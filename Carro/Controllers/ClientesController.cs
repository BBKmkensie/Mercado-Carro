using Carro.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Carro.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;
        public ClientesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult LoginIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginViewModel Lvm)
        {
            var usuarios = _context.tblUsuario.ToList();
            if (usuarios.Count == 0)
            {
                var U = new Usuario();
                U.Name = "Clientes";
                U.Email = "admi@inacap.cl";
                U.Username = "Client";
                U.Rol = "Clientes";

                CreatePasswordHash("12345", out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.tblUsuario.Add(U);
                _context.SaveChanges();


            }

            var us = _context.tblUsuario.Where(u => u.Username.Equals(Lvm.Username)).FirstOrDefault();
            if (us != null)
            {
                //Usuario Encontrado
                if (VerificarPass(Lvm.Password, us.PasswordHash, us.PasswordSalt))
                {
                    //Usuario y contraseña Correctos!
                    var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, us.Username),
                        new Claim(ClaimTypes.NameIdentifier, Lvm.Username),
                        new Claim(ClaimTypes.Role, us.Rol)
                    };

                    //Carnet, Licencia
                    var identity = new ClaimsIdentity(Claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var pricipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pricipal,
                        new AuthenticationProperties { IsPersistent = true }
                        );

                    return RedirectToAction("Index", "Home");

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
        public async Task<IActionResult> EditarUsuario(Guid userid)
        {
            var U = _context.tblUsuario.FirstOrDefault(u => u.UsuarioId.Equals(userid));
            if (U == null) return NotFound();
            EditarUserViewModel Evm = new EditarUserViewModel()
            {
                Email = U.Email,
                Username = U.Username,
                Id = userid,
                Name = U.Name,
                Rol = U.Rol,
            };
            return View(Evm);
        }

        [HttpGet]
      

        [Authorize(Roles = "SuperAdministrador, Administrador")]
        [HttpPost]
        public async Task<IActionResult> EditarUsuario(EditarUserViewModel Evm)
        {
            var U = _context.tblUsuario.FirstOrDefault(u => u.UsuarioId.Equals(Evm.Id));
            if (U == null) return NotFound();

            U.Name = Evm.Name;
            U.Email = Evm.Email;
            U.Username = Evm.Username;

            CreatePasswordHash(Evm.Password, out byte[] passwordHash, out byte[] passwordSalt);
            U.PasswordSalt = passwordSalt;
            U.PasswordHash = passwordHash;

            _context.Update(U);
            await _context.SaveChangesAsync();

            var Rol = User.FindFirst(ClaimTypes.Role).Value;
            if (Rol == "Administrador" || Rol == "SuperAdministrador") return RedirectToAction(nameof(EditarUsuario));
            else return RedirectToAction("LogOut", "Auth");


        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(LoginIn));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }




        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwodSalt)
        {
            //123456 Cliente
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
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }

        }
    }
}
