namespace LocadoraVeiculosApi.DTOs;

public class VeiculoDetalhadoFiltroDto
{
    public int VeiculoId { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int AnoFabricacao { get; set; }
    public int QuilometragemAtual { get; set; }
    public string Fabricante { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
}

public class AluguelPorClienteFiltroDto
{
    public int AluguelId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Veiculo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFimPrevista { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public decimal ValorTotal { get; set; }
}

public class VeiculoSemAluguelFiltroDto
{
    public int VeiculoId { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
}

public class FabricanteComTotalVeiculosFiltroDto
{
    public int FabricanteId { get; set; }
    public string Fabricante { get; set; } = string.Empty;
    public int TotalVeiculos { get; set; }
}
