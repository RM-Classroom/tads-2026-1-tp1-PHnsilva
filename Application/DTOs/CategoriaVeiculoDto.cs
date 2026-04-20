using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.Application.DTOs;

public class CategoriaVeiculoDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 999999.99)]
    public decimal ValorDiariaBase { get; set; }
}
