using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ProyectoFacturacionMVC.Models;

public partial class Articulo
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    [DisplayName("Estado")]
    public bool? Activo { get; set; }

    [NotMapped]
    [DisplayName("Fecha de Ingreso")]
    public DateOnly Fecha { get; set; }
}
