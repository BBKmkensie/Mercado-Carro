using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Carro.Models;
using Microsoft.AspNetCore.Authorization;

namespace Carro.Controllers
{
    [Authorize(Roles = "SuperAdministrador")]   
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
              return View(await _context.tblCategorias.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tblCategorias == null)
            {
                return NotFound();
            }

            var category = await _context.tblCategorias
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "SuperAdministrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdministrador")]
        public async Task<IActionResult> Create([Bind("CategoryId,Descripcion")] Category category)
        {
            if (ModelState.IsValid)
            {

                if(_context.tblCategorias.Where(c=>c.Descripcion == (category.Descripcion)).FirstOrDefault() == null)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe esa categoria");
                    return View(category);
                }


                
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tblCategorias == null)
            {
                return NotFound();
            }

            var category = await _context.tblCategorias.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


       
            // POST: Categories/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Descripcion")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "SuperAdministrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tblCategorias == null)
            {
                return NotFound();
            }

            var category = await _context.tblCategorias
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [Authorize(Roles = "SuperAdministrador")]
        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdministrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tblCategorias == null)
            {
                return Problem("Entity set 'AppDbContext.tblCategorias'  is null.");
            }
            var category = await _context.tblCategorias.FindAsync(id);
            if (category != null)
            {
                _context.tblCategorias.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return _context.tblCategorias.Any(e => e.CategoryId == id);
        }


    }
}
