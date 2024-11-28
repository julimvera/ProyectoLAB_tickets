using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApp_Tickets.Data; 
using WebApp_Tickets.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private const int PageSize = 5; 

        public async Task<IActionResult> Index(string? apellido, string? nombre, int page = 1)
        {
          
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

           
            var tickets = await ticketsQuery
                .OrderBy(t => t.FechaSolicitud)
                .Skip((page - 1) * PageSize) 
                .Take(PageSize) 
                .ToListAsync();

            
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(tickets);
        }


        [Authorize]
        //metodo GET para borrar tickets pendientes 
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

        //metodo POST para borrar tickets pendientes
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El ticket pendiente se ha eliminado correctamente.";
            }

            // Redirigir al Index de HomeController después de eliminar el ticket pendiente
            return RedirectToAction(nameof(Index), "Home");
        }
        [Authorize]
        //metodo GET para editar tickets pendientes
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
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticket.EstadoId);
            return View(ticket);
        }

        //metodo POST para editar tickets pendientes
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
                    TempData["SuccessMessage"] = "El ticket pendiente se ha editado correctamente.";
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
                return RedirectToAction(nameof(Index), "Home");
            }

            ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Id", ticket.EstadoId);
            return View(ticket);
        }


        private bool TicketExists(int id) //devuelve un booleano si existe o no un ticket
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        public IActionResult SobreNosotros() 
        {
            return View();

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
