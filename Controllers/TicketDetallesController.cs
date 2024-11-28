using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp_Tickets.Data;
using WebApp_Tickets.Models;

namespace WebApp_Tickets.Controllers
{
    public class TicketDetallesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketDetallesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketDetalles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketDetalles.Include(t => t.Estado).Include(t => t.Ticket);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketDetalle = await _context.TicketDetalles
                .Include(t => t.Estado)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketDetalle == null)
            {
                return NotFound();
            }

            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Create
        public IActionResult Create()
        {
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id");
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id");
            return View();
        }

        // POST: TicketDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,DescripcionPedido,EstadoId,FechaEstado")] TicketDetalle ticketDetalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticketDetalle.EstadoId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketDetalle.TicketId);
            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketDetalle = await _context.TicketDetalles.FindAsync(id);
            if (ticketDetalle == null)
            {
                return NotFound();
            }
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticketDetalle.EstadoId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketDetalle.TicketId);
            return View(ticketDetalle);
        }

        // POST: TicketDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,DescripcionPedido,EstadoId,FechaEstado")] TicketDetalle ticketDetalle)
        {
            if (id != ticketDetalle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketDetalleExists(ticketDetalle.Id))
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
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticketDetalle.EstadoId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Id", ticketDetalle.TicketId);
            return View(ticketDetalle);
        }

        // GET: TicketDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketDetalle = await _context.TicketDetalles
                .Include(t => t.Estado)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketDetalle == null)
            {
                return NotFound();
            }

            return View(ticketDetalle);
        }

        // POST: TicketDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketDetalle = await _context.TicketDetalles.FindAsync(id);
            if (ticketDetalle != null)
            {
                _context.TicketDetalles.Remove(ticketDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketDetalleExists(int id)
        {
            return _context.TicketDetalles.Any(e => e.Id == id);
        }
    }
}
