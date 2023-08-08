using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFacturacionMVC.Models;

public partial class ArticulosFactura
{
    public int Id { get; set; }

    public int IdFactura { get; set; }

    public int IdArticulo { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public DateOnly Fecha { get; set; }

    [NotMapped]
    public string? ArticuloName { get; set; }
}
