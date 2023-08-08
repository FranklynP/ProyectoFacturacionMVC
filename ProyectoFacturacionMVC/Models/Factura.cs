using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFacturacionMVC.Models;

public partial class Factura
{
    public int Id { get; set; }

    [NotMapped]
    public int NumFactura { get; set; }

    [DisplayName("Cliente")]
    public int IdCliente { get; set; }

    [NotMapped]
    [DisplayName("Cliente")]
    public string? ClienteName { get; set; }

    [DisplayName("Vendedor")]
    public int IdVendedor { get; set; }

    [NotMapped]
    [DisplayName("Vendedor")]
    public string? VendedorName { get; set; }

    public int? Comprobante { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Itbis { get; set; }

    public decimal Total { get; set; }

    public string? Comentario { get; set; }

    [NotMapped]
    [DisplayName("Fecha de Ingreso")]
    public DateOnly FechaCreacion { get; set; }

    [DisplayName("Tipo Factura")]
    public string TipoFactura { get; set; } = null!;

    [NotMapped]
    public List<ArticulosFactura>? FacturaArticulos { get; set; }

}
