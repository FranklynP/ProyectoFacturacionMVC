using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFacturacionMVC.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int Rnc { get; set; }

    public string Cedula { get; set; } = null!;

    [DisplayName("Cuenta Contable")]
    public int CuentaContableId { get; set; }

    [NotMapped]
    public string? CuentaContableDesc { get; set; }

    [NotMapped]
    [DisplayName("Fecha de Ingreso")]
    public DateOnly Fecha { get; set; }
}
