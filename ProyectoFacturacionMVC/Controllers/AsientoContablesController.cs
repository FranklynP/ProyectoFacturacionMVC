using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFacturacionMVC.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoFacturacionMVC.Controllers
{
    public class AsientoContablesController : Controller
    {
        private readonly ProyectoFacturacionDbContext _context;
        private readonly HttpClient _httpClient = new HttpClient();

        public AsientoContablesController(ProyectoFacturacionDbContext context)
        {
            _context = context;

            // Configurar el HttpClient con la base address y los encabezados
            _httpClient.BaseAddress = new Uri("http://129.80.203.120:5000/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: AsientoContables
        public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta)
        {

            var asientosContables = (from ast in _context.AsientoContables
                            join CuentaContableDb in _context.CuentaContables on ast.CuentaDb equals CuentaContableDb.Id
                            join CuentaContableCr in _context.CuentaContables on ast.CuentaCr equals CuentaContableCr.Id
                            select new AsientoContable
                            {
                                Id = ast.Id,
                                Descripcion = ast.Descripcion,
                                Auxiliar = ast.Auxiliar,
                                CuentaDb = ast.CuentaDb,
                                CuentaDbDesc = CuentaContableDb.Descripcion,
                                CuentaCr = ast.CuentaCr,
                                CuentaCrDesc = CuentaContableCr.Descripcion,
                                Monto = ast.Monto,
                                FechaCreacion = ast.FechaCreacion,
                                Estado = ast.Estado,
                                IdFactura = ast.IdFactura,
                            });

            var asientosContablesList = await asientosContables.ToListAsync();

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                var fromDateStr = fechaHasta.Value.ToString("yyyy-MM-dd");
                var toDateStr = fechaHasta.Value.ToString("yyyy-MM-dd");

                asientosContablesList = asientosContablesList.Where(ast => ast.FechaCreacion.ToString("yyyy-MM-dd") == fromDateStr)
                                                        .Where(ast => ast.FechaCreacion.ToString("yyyy-MM-dd") == toDateStr)
                                                        .ToList();
            }

            return (_context.AsientoContables != null) || (asientosContablesList != null) ? 
                          View(asientosContablesList) :
                          Problem("Entity set 'ProyectoFacturacionDbContext.AsientoContables'  is null.");
        }

        // GET: AsientoContables/Contabilizar/5
        public async Task<IActionResult> Contabilizar(int? id)
        {
            if (id == null || _context.AsientoContables == null)
            {
                return NotFound();
            }

            var asientoContable = await (from ast in _context.AsientoContables
                                         join CuentaContableDb in _context.CuentaContables on ast.CuentaDb equals CuentaContableDb.Id
                                         join CuentaContableCr in _context.CuentaContables on ast.CuentaCr equals CuentaContableCr.Id
                                         select new AsientoContable
                                         {
                                             Id = ast.Id,
                                             Descripcion = ast.Descripcion,
                                             Auxiliar = ast.Auxiliar,
                                             CuentaDb = ast.CuentaDb,
                                             CuentaDbDesc = CuentaContableDb.Descripcion,
                                             CuentaCr = ast.CuentaCr,
                                             CuentaCrDesc = CuentaContableCr.Descripcion,
                                             Monto = ast.Monto,
                                             FechaCreacion = ast.FechaCreacion,
                                             Estado = ast.Estado,
                                             IdFactura = ast.IdFactura,
                                         }).FirstOrDefaultAsync(m => m.Id == id);

            if (asientoContable == null)
            {
                return NotFound();
            }
            return View(asientoContable);
        }


        // POST: AsientoContables/Contabilizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contabilizar([Bind("Id,Descripcion,Estado,Auxiliar,CuentaDb,CuentaCr,Monto")] AsientoContable asientoContable)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string endpoint = "post-accounting-entries";

                    // Crear un objeto anonimo con el formato esperado por la API
                    var requestData = new
                    {
                        descripcion = asientoContable.Descripcion,
                        auxiliar = asientoContable.Auxiliar,
                        cuentaDB = asientoContable.CuentaDb,
                        cuentaCR = asientoContable.CuentaCr,
                        monto = asientoContable.Monto
                    };

                    var json = JsonSerializer.Serialize(requestData);

                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    using (var response = await _httpClient.PostAsync(endpoint, content))
                    {
                        response.EnsureSuccessStatusCode(); // Throws an exception if the response status code is not a success code

                        // Procesar la respuesta si es necesario
                        var responseContent = await response.Content.ReadAsStringAsync();

                        var responseResult = JsonDocument.Parse(responseContent).RootElement.GetProperty("exito").GetBoolean();

                        if (responseResult)
                        {
                            // Obtener el asientoContable de la base de datos
                            var asientoExistente = await _context.AsientoContables.FindAsync(asientoContable.Id);
                            asientoExistente.Estado = !(asientoContable.Estado);
                            await _context.SaveChangesAsync();
                        }

                        // Redireccionar a la ventana Index
                        return RedirectToAction("Index");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle HTTP request exceptions, e.g., log the error or display an error message.
                    // You can also return a view to display an error message to the user.
                    ModelState.AddModelError("", $"An error occurred while sending the data to the API. Please try again later. Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions, e.g., log the error or display an error message.
                    ModelState.AddModelError("", $"An unexpected error occurred. Please try again later. Error: {ex.Message}");
                }
            }

            // Si el modelo no es válido, regresar al formulario con los errores mostrados
            return View(asientoContable);
        }





        // GET: AsientoContables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AsientoContables == null)
            {
                return NotFound();
            }

            var asientoContable = await _context.AsientoContables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asientoContable == null)
            {
                return NotFound();
            }

            return View(asientoContable);
        }

        // GET: AsientoContables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AsientoContables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,CuentaDb,CuentaCr,Monto,Estado,FechaCreacion,IdFactura")] AsientoContable asientoContable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asientoContable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(asientoContable);
        }


        // GET: AsientoContables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AsientoContables == null)
            {
                return NotFound();
            }

            var asientoContable = await _context.AsientoContables.FindAsync(id);
            if (asientoContable == null)
            {
                return NotFound();
            }
            return View(asientoContable);
        }

        // POST: AsientoContables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,CuentaDb,CuentaCr,Monto,Estado,FechaCreacion,IdFactura")] AsientoContable asientoContable)
        {
            if (id != asientoContable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asientoContable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsientoContableExists(asientoContable.Id))
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
            return View(asientoContable);
        }

        // GET: AsientoContables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AsientoContables == null)
            {
                return NotFound();
            }

            var asientoContable = await _context.AsientoContables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asientoContable == null)
            {
                return NotFound();
            }

            return View(asientoContable);
        }

        // POST: AsientoContables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AsientoContables == null)
            {
                return Problem("Entity set 'ProyectoFacturacionDbContext.AsientoContables'  is null.");
            }
            var asientoContable = await _context.AsientoContables.FindAsync(id);
            if (asientoContable != null)
            {
                _context.AsientoContables.Remove(asientoContable);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsientoContableExists(int id)
        {
          return (_context.AsientoContables?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
