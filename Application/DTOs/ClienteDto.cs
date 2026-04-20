using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(14, MinimumLength = 11)]
    public string Cpf { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefone { get; set; }
}
