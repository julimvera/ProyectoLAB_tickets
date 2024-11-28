using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApp_Tickets.Models;
using WebApp_Tickets.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;

namespace WebApp_Tickets.Controllers
{
    public class AfiliadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadsFolder = Path.Combine("wwwroot", "uploads");

        public AfiliadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Afiliados   METODO PARA PAGINACION Y PARA FILTRADO POR NOMBRE Y APELLIDO
        public async Task<IActionResult> Index(string? nombre, string? apellido, int page = 1)
        {
       
            const int PageSize = 5;

            
            var afiliados = _context.Afiliados.AsQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                afiliados = afiliados.Where(a => a.Nombres.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(apellido))
            {
                afiliados = afiliados.Where(a => a.Apellido.Contains(apellido));
            }

            
            var totalCount = await afiliados.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            
            var result = await afiliados
                .OrderBy(a => a.Id) 
                .Skip((page - 1) * PageSize) 
                .Take(PageSize) 
                .ToListAsync();

           
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            
            ViewData["Nombre"] = nombre;
            ViewData["Apellido"] = apellido;

           
            return View(result);
        }



        [Authorize]
        // GET: Afiliados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Afiliados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Afiliado afiliado, IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (foto != null && foto.Length > 0)
                    {
                        string filePath = Path.Combine(_uploadsFolder, foto.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await foto.CopyToAsync(stream);
                        }
                        afiliado.Foto = $"/uploads/{foto.FileName}";
                    }

                    // Agregar afiliado a la base de datos
                    _context.Add(afiliado);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["SuccessMessage"] = "El afiliado se ha creado correctamente.";
                    return RedirectToAction(nameof(Index)); // Redirigir a la lista de afiliados
                }
                catch (Exception ex)
                {
                    // Mensaje de error en caso de fallo
                    TempData["ErrorMessage"] = $"Ocurrió un error al crear el afiliado: {ex.Message}";
                }
            }
            return View(afiliado); // En caso de error de validación o fallo en la creación
        }



        [Authorize]
        // GET: Afiliados/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var afiliado = await _context.Afiliados.FindAsync(id);
            if (afiliado == null)
            {
                TempData["ErrorMessage"] = "El afiliado no se encontró.";
                return RedirectToAction(nameof(Index));
            }
            return View(afiliado);
        }

        
        // POST: Afiliados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Afiliado afiliado, IFormFile? nuevaFoto)
        {
            if (id != afiliado.Id)
            {
                TempData["ErrorMessage"] = "El afiliado no existe.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var afiliadoExistente = await _context.Afiliados.FindAsync(id);
                if (afiliadoExistente == null)
                {
                    TempData["ErrorMessage"] = "El afiliado no se encontró.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualiza la foto solo si se subió una nueva
                if (nuevaFoto != null && nuevaFoto.Length > 0)
                {
                    string filePath = Path.Combine(_uploadsFolder, nuevaFoto.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await nuevaFoto.CopyToAsync(stream);
                    }
                    afiliadoExistente.Foto = $"/uploads/{nuevaFoto.FileName}";
                }

                // Actualiza las demás propiedades
                afiliadoExistente.Apellido = afiliado.Apellido;
                afiliadoExistente.Nombres = afiliado.Nombres;
                afiliadoExistente.DNI = afiliado.DNI;
                afiliadoExistente.FechaNacimiento = afiliado.FechaNacimiento;

                try
                {
                    _context.Update(afiliadoExistente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Afiliado actualizado correctamente.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al actualizar el afiliado.";
                }

                return RedirectToAction(nameof(Index));
            }
            return View(afiliado);
        }

        [Authorize]
        // GET: Afiliados/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var afiliado = await _context.Afiliados.FindAsync(id);
            if (afiliado == null)
            {
                TempData["ErrorMessage"] = "El afiliado no se encontró.";
                return RedirectToAction(nameof(Index));
            }
            return View(afiliado);
        }

        // POST: Afiliados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var afiliado = await _context.Afiliados.FindAsync(id);
            if (afiliado != null)
            {
                try
                {
                    _context.Afiliados.Remove(afiliado);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Afiliado eliminado correctamente.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al eliminar el afiliado.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo encontrar el afiliado para eliminar.";
            }

            return RedirectToAction(nameof(Index));
        }



      
        [Authorize]
        // GET: umestra la vista Afiliados/ImportarDesdeExcel 
        public IActionResult ImportarDesdeExcel()
        {
            return View();
        }

        // Acción POST para procesar el archivo Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarDesdeExcel(IFormFile archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoExcel.CopyToAsync(memoryStream);

                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; 
                        int totalRows = worksheet.Dimension.Rows;

                        for (int row = 2; row <= totalRows; row++) // Comienza desde la fila 2 (para saltarse los encabezados)
                        {
                            // Extraemos los datos de la fila
                            var apellido = worksheet.Cells[row, 1].Text;
                            var nombres = worksheet.Cells[row, 2].Text;
                            var dni = worksheet.Cells[row, 3].Text;
                            var fechaNacimientoStr = worksheet.Cells[row, 4].Text; // Fecha Nacimiento
                            var foto = worksheet.Cells[row, 5].Text; // Foto

                            // Verificar si al menos el apellido, nombres y DNI están presentes
                            if (string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(dni))
                            {
                                continue; // Saltar esta fila si no tiene datos completos
                            }

                            var afiliado = new Afiliado
                            {
                                Apellido = apellido,
                                Nombres = nombres,
                                DNI = dni,
                                Foto = foto
                            };

                            // Validar y convertir la fecha de nacimiento
                            if (!string.IsNullOrEmpty(fechaNacimientoStr))
                            {
                               
                                if (DateTime.TryParse(fechaNacimientoStr, out DateTime fechaNacimiento))
                                {
                                    afiliado.FechaNacimiento = fechaNacimiento;
                                }
                                else
                                {
                                    // Si no es una fecha válida, asignar un valor por defecto o manejar el error
                                    afiliado.FechaNacimiento = DateTime.MinValue; 
                                }
                            }
                            

                           
                            _context.Afiliados.Add(afiliado);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                
                TempData["SuccessMessage"] = "Afiliados importados con éxito.";

                return RedirectToAction(nameof(Index)); // Redirigir a la lista de afiliados
            }

           
            TempData["ErrorMessage"] = "No se ha seleccionado un archivo válido.";

            return RedirectToAction(nameof(ImportarDesdeExcel));
        }

        [Authorize]
        // GET: Afiliados/CargarManualmente
        public IActionResult CargarManualmente()
        {
            return View();
        }
        //POST para Afiliados/CargarManualmente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarManualmente(Afiliado afiliado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(afiliado);
                await _context.SaveChangesAsync();
                
                // Mensaje de éxito 
                TempData["SuccessMessage"] = "El afiliado se ha cargado correctamente.";

                return RedirectToAction(nameof(Index));
            }


            return View(afiliado);
        }

      
    }
}
