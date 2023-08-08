using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFacturacionMVC.Models;

public partial class AsientoContable
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    [DisplayName("Cuenta Debito")]
    public int CuentaDb { get; set; }

    [NotMapped]
    [DisplayName("Cuenta Debito")]
    public string? CuentaDbDesc { get; set; }

    [DisplayName("Cuenta Credito")]
    public int CuentaCr { get; set; }

    [NotMapped]
    [DisplayName("Cuenta Credito")]
    public string? CuentaCrDesc { get; set; }

    public decimal Monto { get; set; }

    [NotMapped]
    [DisplayName("Estado")]
    public bool Estado { get; set; }

    [NotMapped]
    [DisplayName("Fecha de Ingreso")]
    public DateOnly FechaCreacion { get; set; }

    public int IdFactura { get; set; }

    [NotMapped]
    public int Auxiliar { get; set; } = 3;
}
