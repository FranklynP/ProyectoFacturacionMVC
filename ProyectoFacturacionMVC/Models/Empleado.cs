using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFacturacionMVC.Models;

public partial class Empleado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int Comision { get; set; }

    [DisplayName("Estado")]
    public bool? Activo { get; set; }

    [NotMapped]
    [DisplayName("Fecha de Ingreso")]
    public DateOnly Fecha { get; set; }
}
