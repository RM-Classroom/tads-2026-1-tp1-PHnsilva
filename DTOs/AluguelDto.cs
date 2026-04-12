using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculosApi.DTOs;

public class AluguelDto
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue)]
    public int VeiculoId { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFimPrevista { get; set; }

    public DateTime? DataDevolucao { get; set; }

    [Range(0, int.MaxValue)]
    public int QuilometragemInicial { get; set; }

    [Range(0, int.MaxValue)]
    public int? QuilometragemFinal { get; set; }

    [Range(0, 999999.99)]
    public decimal ValorDiaria { get; set; }

    [Range(0, 99999999.99)]
    public decimal ValorTotal { get; set; }
}
