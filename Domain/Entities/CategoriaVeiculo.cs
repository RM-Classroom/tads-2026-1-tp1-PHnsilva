using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.Domain.Entities;

public class CategoriaVeiculo
{
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 999999.99)]
    public decimal ValorDiariaBase { get; set; }

    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
