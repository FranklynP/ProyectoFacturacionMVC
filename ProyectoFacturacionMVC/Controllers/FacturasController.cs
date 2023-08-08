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
    public class FacturasController : Controller
    {
        private readonly ProyectoFacturacionDbContext _context;

        public FacturasController(ProyectoFacturacionDbContext context)
        {
            _context = context;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var facturas = await (from fta in _context.Facturas
                            join Cliente in _context.Clientes on fta.IdCliente equals Cliente.Id
                            join Empleado in _context.Empleados on fta.IdVendedor equals Empleado.Id
                            select new Factura
                            {
                                Id = fta.Id,
                                NumFactura = fta.NumFactura,
                                IdCliente = fta.IdCliente,
                                ClienteName = Cliente.Nombre + " " + Cliente.Apellido,
                                IdVendedor = fta.IdVendedor,
                                VendedorName = Empleado.Nombre + " " + Empleado.Apellido,
                                Comprobante = fta.Comprobante,
                                Subtotal = fta.Subtotal,
                                Itbis = fta.Itbis,
                                Total = fta.Total,
                                Comentario = fta.Comentario,
                                FechaCreacion = fta.FechaCreacion
                            }).ToListAsync();

            return facturas != null ?
                View(facturas) :
                Problem("Entity set 'ProyectoFacturacionDbContext.Facturas' is null.");
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            // var factura = await _context.Facturas
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var factura = await (from fta in _context.Facturas
                                  join Cliente in _context.Clientes on fta.IdCliente equals Cliente.Id
                                  join Empleado in _context.Empleados on fta.IdVendedor equals Empleado.Id
                                  select new Factura
                                  {
                                      Id = fta.Id,
                                      NumFactura = fta.NumFactura,
                                      IdCliente = fta.IdCliente,
                                      ClienteName = Cliente.Nombre + " " + Cliente.Apellido,
                                      IdVendedor = fta.IdVendedor,
                                      VendedorName = Empleado.Nombre + " " + Empleado.Apellido,
                                      Comprobante = fta.Comprobante,
                                      Subtotal = fta.Subtotal,
                                      Itbis = fta.Itbis,
                                      Total = fta.Total,
                                      Comentario = fta.Comentario,
                                      FechaCreacion = fta.FechaCreacion
                                  }).FirstOrDefaultAsync(m => m.Id == id);

            if (factura == null)
            {
                return NotFound();
            }

            List<ArticulosFactura> articulosFactura = (from d in _context.ArticulosFacturas
                                      join Articulo in _context.Articulos on d.IdArticulo equals Articulo.Id
                                      where d.IdFactura == id
                                      select new ArticulosFactura
                                      {
                                          Id = d.Id,
                                          ArticuloName = Articulo.Descripcion,
                                          PrecioUnitario = d.PrecioUnitario,
                                          Cantidad = d.Cantidad,
                                      }).ToList();

            factura.FacturaArticulos = articulosFactura;

            return View(factura);
        }

        // GET: Facturas/Create
        public IActionResult Create()
        {
            List<Cliente> clientes = (from d in _context.Clientes
                                      select new Cliente
                                      {
                                          Id = d.Id,
                                          Nombre = d.Nombre + " " + d.Apellido
                                      }).ToList();

            List<Empleado> empleados = (from d in _context.Empleados
                                        select new Empleado
                                        {
                                            Id = d.Id,
                                            Nombre = d.Nombre + " " + d.Apellido
                                        }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.clientes_list = new SelectList(clientes, "Id", "Nombre");

            // Pasar la lista de cuentas contables a la vista
            ViewBag.empleados_list = new SelectList(empleados, "Id", "Nombre");

            ViewBag.tipos_factura_list = new SelectList(new[]
                    {
                        new { Value = "venta", Text = "Venta" },
                        new { Value = "compra", Text = "Compra" }
                    }, "Value", "Text");

            return View();
        }

        // POST: Facturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumFactura,IdCliente,IdVendedor,Comprobante,Subtotal,Itbis,Total,Comentario,FechaCreacion,FacturaArticulos,TipoFactura")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                _context.Add(factura);
                await _context.SaveChangesAsync();

                foreach (var articulo in factura.FacturaArticulos)
                {
                    ArticulosFactura articulosFactura = new ArticulosFactura();

                    articulosFactura.IdFactura = factura.Id;
                    articulosFactura.IdArticulo = articulo.IdArticulo;
                    articulosFactura.Cantidad = articulo.Cantidad;
                    articulosFactura.PrecioUnitario = articulo.PrecioUnitario;

                    _context.Add(articulosFactura);

                    await _context.SaveChangesAsync();

                    // Obtener el articulo existente de la base de datos
                    var articuloAlmacen = await _context.Articulos.FindAsync(articulosFactura.IdArticulo);

                    Int32 stockFinal = articuloAlmacen.Stock - articulo.Cantidad;

                    articuloAlmacen.Stock = (stockFinal < 0) ? 0 : stockFinal;

                    await _context.SaveChangesAsync();

                }
                
                CreateAsientoContable(factura);

                return RedirectToAction(nameof(Index));
            }

            List<Cliente> clientes = (from d in _context.Clientes
                                      select new Cliente
                                      {
                                          Id = d.Id,
                                          Nombre = d.Nombre + " " + d.Apellido
                                      }).ToList();

            List<Empleado> empleados = (from d in _context.Empleados
                                        select new Empleado
                                        {
                                            Id = d.Id,
                                            Nombre = d.Nombre + " " + d.Apellido
                                        }).ToList();

            // Pasar la lista de clientes a la vista
            ViewBag.clientes_list = new SelectList(clientes, "Id", "Nombre");

            // Pasar la lista de vendedores a la vista
            ViewBag.empleados_list = new SelectList(empleados, "Id", "Nombre");

            ViewBag.tipos_factura_list = new SelectList(new[]
                    {
                        new { Value = "venta", Text = "Venta" },
                        new { Value = "compra", Text = "Compra" }
                    }, "Value", "Text");

            return View(factura);
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            List<ArticulosFactura> articulosFactura = (from d in _context.ArticulosFacturas
                                                       join Articulo in _context.Articulos on d.IdArticulo equals Articulo.Id
                                                       where d.IdFactura == id
                                                       select new ArticulosFactura
                                                       {
                                                           IdArticulo = d.IdArticulo,
                                                           ArticuloName = Articulo.Descripcion,
                                                           PrecioUnitario = d.PrecioUnitario,
                                                           Cantidad = d.Cantidad,
                                                       }).ToList();

            factura.FacturaArticulos = articulosFactura;

            List<Cliente> clientes = (from d in _context.Clientes
                                      select new Cliente
                                      {
                                          Id = d.Id,
                                          Nombre = d.Nombre + " " + d.Apellido
                                      }).ToList();

            List<Empleado> empleados = (from d in _context.Empleados
                                        select new Empleado
                                        {
                                            Id = d.Id,
                                            Nombre = d.Nombre + " " + d.Apellido
                                        }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.clientes_list = new SelectList(clientes, "Id", "Nombre");

            // Pasar la lista de cuentas contables a la vista
            ViewBag.empleados_list = new SelectList(empleados, "Id", "Nombre");

            ViewBag.tipos_factura_list = new SelectList(new[]
                    {
                        new { Value = "venta", Text = "Venta" },
                        new { Value = "compra", Text = "Compra" }
                    }, "Value", "Text");

            // Pasar la lista de articulos a la vista
            List<Articulo> articulos = _context.Articulos.Where(m => m.Stock > 0).ToList();
            ViewBag.articulos_list = articulos;

            return View(factura);
        }

        // POST: Facturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumFactura,IdCliente,IdVendedor,Comprobante,Subtotal,Itbis,Total,Comentario,FacturaArticulos,TipoFactura")] Factura factura)
        {
            if (id != factura.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el cliente existente de la base de datos
                    var facturaExistente = await _context.Facturas.FindAsync(factura.Id);

                    // Actualizar solo las propiedades necesarias
                    facturaExistente.NumFactura = factura.NumFactura;
                    facturaExistente.IdCliente = factura.IdCliente;
                    facturaExistente.IdVendedor = factura.IdVendedor;
                    facturaExistente.Comprobante = factura.Comprobante;
                    facturaExistente.Subtotal = factura.Subtotal;
                    facturaExistente.Total = factura.Total;
                    facturaExistente.Comentario = factura.Comentario;
                    facturaExistente.TipoFactura = factura.TipoFactura;

                    // Obteniendo Articulos de la Factura actuales y eliminandolos
                    var articulosActuales = _context.ArticulosFacturas.Where(articulo => articulo.IdFactura == factura.Id);
                    _context.ArticulosFacturas.RemoveRange(articulosActuales);

                    await _context.SaveChangesAsync();

                    // Recorriendo los "nuevos" articulos y agregandolos
                    foreach (var articulo in factura.FacturaArticulos)
                    {
                        ArticulosFactura articulosFactura = new ArticulosFactura();

                        articulosFactura.IdFactura = factura.Id;
                        articulosFactura.IdArticulo = articulo.IdArticulo;
                        articulosFactura.Cantidad = articulo.Cantidad;
                        articulosFactura.PrecioUnitario = articulo.PrecioUnitario;

                        _context.Add(articulosFactura);

                        await _context.SaveChangesAsync();

                        // Obteniendo el articulo existente de la base de datos y reduciendo su "stock"
                        var articuloAlmacen = await _context.Articulos.FindAsync(articulosFactura.IdArticulo);

                        Int32 stockFinal = articuloAlmacen.Stock - articulo.Cantidad;

                        articuloAlmacen.Stock = (stockFinal < 0) ? 0 : stockFinal;

                        await _context.SaveChangesAsync();
                    }

                    UpdateAsientoContable(facturaExistente);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.Id))
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


            List<Cliente> clientes = (from d in _context.Clientes
                                      select new Cliente
                                      {
                                          Id = d.Id,
                                          Nombre = d.Nombre + " " + d.Apellido
                                      }).ToList();

            List<Empleado> empleados = (from d in _context.Empleados
                                        select new Empleado
                                        {
                                            Id = d.Id,
                                            Nombre = d.Nombre + " " + d.Apellido
                                        }).ToList();

            // Pasar la lista de cuentas contables a la vista
            ViewBag.clientes_list = new SelectList(clientes, "Id", "Nombre");

            // Pasar la lista de cuentas contables a la vista
            ViewBag.empleados_list = new SelectList(empleados, "Id", "Nombre");

            ViewBag.tipos_factura_list = new SelectList(new[]
                    {
                        new { Value = "venta", Text = "Venta" },
                        new { Value = "compra", Text = "Compra" }
                    }, "Value", "Text");

            return View(factura);
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facturas == null)
            {
                return Problem("Entity set 'ProyectoFacturacionDbContext.Facturas'  is null.");
            }
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
          return (_context.Facturas?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public void CreateAsientoContable(Factura factura)
        {
            if (factura != null)
            {
                var cliente = _context.Clientes.FirstOrDefault(m => m.Id == factura.IdCliente);

                if (factura.TipoFactura == "venta")
                {

                    AsientoContable asientoContable = new AsientoContable();

                    asientoContable.IdFactura = factura.Id;
                    asientoContable.Descripcion = "Asiento de Facturacion - Venta " + cliente.Nombre + " " + cliente.Apellido;
                    asientoContable.CuentaDb = 12;
                    asientoContable.CuentaCr = 13;
                    asientoContable.Monto = factura.Total;

                    _context.Add(asientoContable);
                    _context.SaveChanges();

                }
                else if (factura.TipoFactura == "compra")
                {

                    AsientoContable asientoContable = new AsientoContable();

                    asientoContable.IdFactura = factura.Id;
                    asientoContable.Descripcion = "Asiento de Facturacion - Compra " + cliente.Nombre + " " + cliente.Apellido;
                    asientoContable.CuentaDb = 5;
                    asientoContable.CuentaCr = 80;
                    asientoContable.Monto = factura.Total;

                    _context.Add(asientoContable);
                    _context.SaveChanges();

                }
            }
        }


        public void UpdateAsientoContable(Factura factura)
        {
            if (factura != null)
            {
                var cliente = _context.Clientes.FirstOrDefault(m => m.Id == factura.IdCliente);

                var asientoContableExistente = _context.AsientoContables.FirstOrDefault(m => m.IdFactura == factura.Id);

                if (factura.TipoFactura == "venta")
                {
                    asientoContableExistente.Descripcion = "Asiento de Facturacion - Venta " + cliente.Nombre + " " + cliente.Apellido;
                    asientoContableExistente.CuentaDb = 12;
                    asientoContableExistente.CuentaCr = 13;
                    asientoContableExistente.Monto = factura.Total;
                    asientoContableExistente.Estado = false;

                    _context.SaveChanges();

                }
                else if (factura.TipoFactura == "compra")
                {
                    asientoContableExistente.Descripcion = "Asiento de Facturacion - Compra " + cliente.Nombre + " " + cliente.Apellido;
                    asientoContableExistente.CuentaDb = 5;
                    asientoContableExistente.CuentaCr = 80;
                    asientoContableExistente.Monto = factura.Total;
                    asientoContableExistente.Estado = false;

                    _context.SaveChanges();

                }
            }
        }
    }
}
