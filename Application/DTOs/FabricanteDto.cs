using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.Application.DTOs;

public class FabricanteDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60)]
    public string? PaisOrigem { get; set; }
}
