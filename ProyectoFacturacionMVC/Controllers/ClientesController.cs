using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFacturacionMVC.Models;

namespace ProyectoFacturacionMVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ProyectoFacturacionDbContext _context;

        public ClientesController(ProyectoFacturacionDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = (from clt in _context.Clientes
                               join CuentaContable in _context.CuentaContables on clt.CuentaContableId equals CuentaContable.Id
                               select new Cliente
                               {
                                   Id = clt.Id,
                                   Nombre = clt.Nombre,
                                   Apellido = clt.Apellido,
                                   Rnc = clt.Rnc,
                                   Cedula = clt.Cedula,
                                   Fecha = clt.Fecha,
                                   CuentaContableId = clt.CuentaContableId,
                                   CuentaContableDesc = CuentaContable.Descripcion,
                               }).ToListAsync();

            return (_context.Clientes != null) || (clientes != null) ? 
                          View(await clientes) :
                          Problem("Entity set 'ProyectoFacturacionDbContext.Clientes'  is null.");
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            List<CuentaContable> cuentas_contables = (from d in _context.CuentaContables
                                                     select new CuentaContable
                                                     {
                                                         Id = d.Id,
                                                         Descripcion = d.Descripcion
                                                     }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.cuentas_contables_list = new SelectList(cuentas_contables, "Id", "Descripcion");

            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Rnc,Cedula,CuentaContableId,Fecha")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            List<CuentaContable> cuentas_contables = (from d in _context.CuentaContables
                                                      select new CuentaContable
                                                      {
                                                          Id = d.Id,
                                                          Descripcion = d.Descripcion
                                                      }).ToList(); ;

            // Pasar la lista de cuentas contables a la vista
            ViewBag.cuentas_contables_list = new SelectList(cuentas_contables, "Id", "Descripcion");

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            List<CuentaContable> cuentas_contables = (from d in _context.CuentaContables
                                                      select new CuentaContable
                                                      {
                                                          Id = d.Id,
                                                          Descripcion = d.Descripcion
                                                      }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.cuentas_contables_list = new SelectList(cuentas_contables, "Id", "Descripcion", cliente.CuentaContableId);

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Rnc,Cedula,CuentaContableId")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el cliente existente de la base de datos
                    var clienteExistente = await _context.Clientes.FindAsync(cliente.Id);

                    // Actualizar solo las propiedades necesarias
                    clienteExistente.Nombre = cliente.Nombre;
                    clienteExistente.Apellido = cliente.Apellido;
                    clienteExistente.Rnc = cliente.Rnc;
                    clienteExistente.Cedula = cliente.Cedula;
                    clienteExistente.CuentaContableId = cliente.CuentaContableId;

                    // Guardar los cambios en la base de datos

                    //_context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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

            // En caso de error retornar a la vista para visualizar los detalles del inconveniente
            var clienteOld = await _context.Clientes.FindAsync(id);
            if (clienteOld == null)
            {
                return NotFound();
            }
            List<CuentaContable> cuentas_contables = (from d in _context.CuentaContables
                                                      select new CuentaContable
                                                      {
                                                          Id = d.Id,
                                                          Descripcion = d.Descripcion
                                                      }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.cuentas_contables_list = new SelectList(cuentas_contables, "Id", "Descripcion", clienteOld.CuentaContableId);

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'ProyectoFacturacionDbContext.Clientes'  is null.");
            }
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
          return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
