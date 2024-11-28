using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp_Tickets.Data;
using Microsoft.AspNetCore.Authorization;
using WebApp_Tickets.Models;

namespace WebApp_Tickets.Controllers
{
    //[Authorize]  // con esto solo los usuarios autenticadospueden acceder a Crear, Editar y Borrar 
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? apellido, string? nombre, int page = 1)
        {
            const int PageSize = 5; 

            var ticketsQuery = _context.Tickets
                .Include(t => t.Afiliado) 
                .Include(t => t.Estado) 
                .Include(t => t.TicketDetalles) 
                .Where(t => t.EstadoId == 1); 

            
            if (!string.IsNullOrEmpty(apellido))
            {
                ticketsQuery = ticketsQuery.Where(t => t.Afiliado != null && t.Afiliado.Apellido.Contains(apellido));
            }

            
            if (!string.IsNullOrEmpty(nombre))
            {
                ticketsQuery = ticketsQuery.Where(t => t.Afiliado != null && t.Afiliado.Nombres.Contains(nombre));
            }

          
            var totalCount = await ticketsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            
            page = page < 1 ? 1 : page;

            
            var tickets = await ticketsQuery
                .OrderBy(t => t.FechaSolicitud) 
                .Skip((page - 1) * PageSize) 
                .Take(PageSize)
                .ToListAsync();

           
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["Apellido"] = apellido;

            return View(tickets);
        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Afiliado)
                .Include(t => t.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }


            return View(ticket);
        }


        [Authorize]
        // GET: Tickets/Create
        [Authorize]
        public IActionResult Create()
        {
           
            ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id"); 
                                                                                           
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion"); 
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,AfiliadoId,FechaSolicitud,Observacion,EstadoId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(ticket);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito al guardar el ticket
                    TempData["SuccessMessage"] = "Ticket creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Mensaje de error
                    TempData["ErrorMessage"] = "Ocurrió un error al crear el ticket: " + ex.Message;
                }
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                TempData["ErrorMessage"] = "Los datos del formulario no son válidos.";
                ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Apellido");
                ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion");
                return View(ticket);
            }
            else
            {
                // Mensaje de error en caso de que el modelo no sea válido
                TempData["ErrorMessage"] = "Los datos del formulario no son válidos.";
            }

            // Recargar las listas en caso de error
            ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Apellido");
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion");

            return View(ticket);
        }



        [Authorize]
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AfiliadoId,FechaSolicitud,Observacion,EstadoId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "El ticket se ha editado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticket.EstadoId);
            return View(ticket);
        }


        [Authorize]
        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Afiliado)
                .Include(t => t.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El ticket se ha eliminado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }



        private bool TicketExists(int id) //devuelve un booleano si existe o no un ticket
        {
            return _context.Tickets.Any(e => e.Id == id);
        }





    }

}
