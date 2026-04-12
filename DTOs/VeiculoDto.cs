using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.DTOs;

public class VeiculoDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int AnoFabricacao { get; set; }

    [Range(0, int.MaxValue)]
    public int QuilometragemAtual { get; set; }

    [Required]
    [StringLength(10)]
    public string Placa { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int FabricanteId { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaVeiculoId { get; set; }
}
