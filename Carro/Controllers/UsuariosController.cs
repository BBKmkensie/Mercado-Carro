using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Carro.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Recursos_Humanos.Controllers
{
    //[Authorize(Roles = "SuperAdministrador, Administrador, Asistente de RRHH, Jefe de RRHH")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
       //[Authorize(Roles = "SuperAdministrador, Administrador")]
        public IActionResult GestionUsuarios()
        {
            return View(_context.tblUsuario.ToList());
        }

        [Authorize(Roles = "SuperAdministrador, Administrador")]
        [HttpGet]
        public IActionResult CrearUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CrearUser(CrearUserViewModel Uvm)
        {
            if (ModelState.IsValid)

            {
                if (_context.tblUsuario.Where(c => c.Username == (Uvm.Username) || c.Email.Equals(Uvm.Email)).FirstOrDefault() == null)
                {
                Usuario U = new Usuario();
                U.Name = Uvm.Name;
                U.Email = Uvm.Email;
                U.Username = Uvm.Username;
                U.Rol = Uvm.Rol;

                CreatePaswordHash(Uvm.Password, out byte[] passwordHash, out byte[] passwordSalt);
                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;


                _context.tblUsuario.Add(U);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GestionUsuarios));
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe ese Dato");
                    return View(Uvm);
                }
                   

            }
            else
            {
                ModelState.AddModelError("", "Error al Crear");
                return View(Uvm);

            }

        }
        //[Authorize(Roles = "SuperAdministrador, Administrador, Asistente de RRHH, Jefe de RRHH")]
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(Guid userid)
        {
            var U = _context.tblUsuario.FirstOrDefault(u => u.UsuarioId.Equals(userid));
            if(U == null) return NotFound();
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

        //[Authorize(Roles = "SuperAdministrador, Administrador, Asistente de RRHH, Jefe de RRHH")]
        [HttpPost]
        public async Task<IActionResult> EditarUsuario(EditarUserViewModel Evm)
        {
            var U = _context.tblUsuario.FirstOrDefault(u => u.UsuarioId.Equals(Evm.Id));
            if (U == null) return NotFound();

            U.Name = Evm.Name;
            U.Email = Evm.Email;
            U.Username = Evm.Username;

            CreatePaswordHash(Evm.Password, out byte[] passwordHash, out byte[] passwordSalt);
            U.PasswordSalt = passwordSalt;
            U.PasswordHash = passwordHash;


            _context.Update(U);
            await _context.SaveChangesAsync();

            var Rol = User.FindFirst(ClaimTypes.Role).Value;
            if (Rol == "Administrador" || Rol == "SuperAdministrador") return RedirectToAction(nameof(GestionUsuarios));
            else return RedirectToAction("LogOut", "Auth");

            
        }

        [Authorize(Roles = "SuperAdministrador, Administrador")]
        [HttpGet]
        public async Task<IActionResult> EliminarUsuario(Guid Id)
        {
            var U = _context.tblUsuario.FirstOrDefault(u => u.UsuarioId.Equals(Id));
            if (U == null)return NotFound();
            else
            {
                _context.Remove(U);
                await _context.SaveChangesAsync();
            }

           
            return RedirectToAction("GestionUsuarios");
        }


        private void CreatePaswordHash(string password, out byte[] passwordHash, out byte[] paswordSalt)
        {
            //administrador 123456
            using (var hmac = new HMACSHA512())
            {
                paswordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
