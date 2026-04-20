using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.Domain.Entities;

public class Veiculo
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

    public int FabricanteId { get; set; }
    public Fabricante? Fabricante { get; set; }

    public int CategoriaVeiculoId { get; set; }
    public CategoriaVeiculo? CategoriaVeiculo { get; set; }

    public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
}
