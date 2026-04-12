using LocadoraVeiculosApi.Data;
using LocadoraVeiculosApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FiltrosController : ControllerBase
{
    private readonly ApplicationContext _context;

    public FiltrosController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet("veiculos-por-fabricante/{fabricanteId:int}")]
    public async Task<ActionResult<IEnumerable<VeiculoDetalhadoFiltroDto>>> GetVeiculosPorFabricante(int fabricanteId)
    {
        var query = from veiculo in _context.Veiculos
                    join fabricante in _context.Fabricantes on veiculo.FabricanteId equals fabricante.Id
                    join categoria in _context.CategoriasVeiculo on veiculo.CategoriaVeiculoId equals categoria.Id
                    where fabricante.Id == fabricanteId
                    select new VeiculoDetalhadoFiltroDto
                    {
                        VeiculoId = veiculo.Id,
                        Modelo = veiculo.Modelo,
                        Placa = veiculo.Placa,
                        AnoFabricacao = veiculo.AnoFabricacao,
                        QuilometragemAtual = veiculo.QuilometragemAtual,
                        Fabricante = fabricante.Nome,
                        Categoria = categoria.Nome
                    };

        return Ok(await query.ToListAsync());
    }

    [HttpGet("alugueis-por-cliente/{clienteId:int}")]
    public async Task<ActionResult<IEnumerable<AluguelPorClienteFiltroDto>>> GetAlugueisPorCliente(int clienteId)
    {
        var query = from aluguel in _context.Alugueis
                    join cliente in _context.Clientes on aluguel.ClienteId equals cliente.Id
                    join veiculo in _context.Veiculos on aluguel.VeiculoId equals veiculo.Id
                    where cliente.Id == clienteId
                    select new AluguelPorClienteFiltroDto
                    {
                        AluguelId = aluguel.Id,
                        Cliente = cliente.Nome,
                        Cpf = cliente.Cpf,
                        Veiculo = veiculo.Modelo,
                        Placa = veiculo.Placa,
                        DataInicio = aluguel.DataInicio,
                        DataFimPrevista = aluguel.DataFimPrevista,
                        DataDevolucao = aluguel.DataDevolucao,
                        ValorTotal = aluguel.ValorTotal
                    };

        return Ok(await query.ToListAsync());
    }

    [HttpGet("veiculos-por-categoria/{categoriaId:int}")]
    public async Task<ActionResult<IEnumerable<VeiculoDetalhadoFiltroDto>>> GetVeiculosPorCategoria(int categoriaId)
    {
        var query = from veiculo in _context.Veiculos
                    join fabricante in _context.Fabricantes on veiculo.FabricanteId equals fabricante.Id
                    join categoria in _context.CategoriasVeiculo on veiculo.CategoriaVeiculoId equals categoria.Id
                    where categoria.Id == categoriaId
                    select new VeiculoDetalhadoFiltroDto
                    {
                        VeiculoId = veiculo.Id,
                        Modelo = veiculo.Modelo,
                        Placa = veiculo.Placa,
                        AnoFabricacao = veiculo.AnoFabricacao,
                        QuilometragemAtual = veiculo.QuilometragemAtual,
                        Fabricante = fabricante.Nome,
                        Categoria = categoria.Nome
                    };

        return Ok(await query.ToListAsync());
    }

    [HttpGet("veiculos-sem-aluguel")]
    public async Task<ActionResult<IEnumerable<VeiculoSemAluguelFiltroDto>>> GetVeiculosSemAluguel()
    {
        var query = from veiculo in _context.Veiculos
                    join fabricante in _context.Fabricantes on veiculo.FabricanteId equals fabricante.Id
                    join aluguel in _context.Alugueis on veiculo.Id equals aluguel.VeiculoId into alugueis
                    from aluguel in alugueis.DefaultIfEmpty()
                    where aluguel == null
                    select new VeiculoSemAluguelFiltroDto
                    {
                        VeiculoId = veiculo.Id,
                        Modelo = veiculo.Modelo,
                        Placa = veiculo.Placa,
                        Fabricante = fabricante.Nome
                    };

        return Ok(await query.ToListAsync());
    }

    [HttpGet("fabricantes-com-total-veiculos")]
    public async Task<ActionResult<IEnumerable<FabricanteComTotalVeiculosFiltroDto>>> GetFabricantesComTotalVeiculos()
    {
        var query = from fabricante in _context.Fabricantes
                    join veiculo in _context.Veiculos on fabricante.Id equals veiculo.FabricanteId into veiculos
                    from veiculo in veiculos.DefaultIfEmpty()
                    group veiculo by new { fabricante.Id, fabricante.Nome } into grouped
                    select new FabricanteComTotalVeiculosFiltroDto
                    {
                        FabricanteId = grouped.Key.Id,
                        Fabricante = grouped.Key.Nome,
                        TotalVeiculos = grouped.Count(x => x != null)
                    };

        return Ok(await query.OrderBy(x => x.Fabricante).ToListAsync());
    }
}
