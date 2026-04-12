using LocadoraVeiculosApi.Data;
using LocadoraVeiculosApi.DTOs;
using LocadoraVeiculosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlugueisController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AlugueisController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var alugueis = await _context.Alugueis
            .Include(x => x.Cliente)
            .Include(x => x.Veiculo)
            .OrderByDescending(x => x.DataInicio)
            .Select(x => new
            {
                x.Id,
                x.ClienteId,
                Cliente = x.Cliente!.Nome,
                x.VeiculoId,
                Veiculo = x.Veiculo!.Modelo,
                x.DataInicio,
                x.DataFimPrevista,
                x.DataDevolucao,
                x.QuilometragemInicial,
                x.QuilometragemFinal,
                x.ValorDiaria,
                x.ValorTotal
            })
            .ToListAsync();

        return Ok(alugueis);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var aluguel = await _context.Alugueis
            .Include(x => x.Cliente)
            .Include(x => x.Veiculo)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.ClienteId,
                Cliente = x.Cliente!.Nome,
                x.VeiculoId,
                Veiculo = x.Veiculo!.Modelo,
                x.DataInicio,
                x.DataFimPrevista,
                x.DataDevolucao,
                x.QuilometragemInicial,
                x.QuilometragemFinal,
                x.ValorDiaria,
                x.ValorTotal
            })
            .FirstOrDefaultAsync();

        return aluguel is null ? NotFound() : Ok(aluguel);
    }

    [HttpPost]
    public async Task<ActionResult<AluguelDto>> Create(AluguelDto dto)
    {
        var validation = await ValidateRental(dto);
        if (validation is not null) return validation;

        var aluguel = new Aluguel
        {
            ClienteId = dto.ClienteId,
            VeiculoId = dto.VeiculoId,
            DataInicio = dto.DataInicio,
            DataFimPrevista = dto.DataFimPrevista,
            DataDevolucao = dto.DataDevolucao,
            QuilometragemInicial = dto.QuilometragemInicial,
            QuilometragemFinal = dto.QuilometragemFinal,
            ValorDiaria = dto.ValorDiaria,
            ValorTotal = dto.ValorTotal
        };

        _context.Alugueis.Add(aluguel);
        await _context.SaveChangesAsync();
        dto.Id = aluguel.Id;
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AluguelDto dto)
    {
        var aluguel = await _context.Alugueis.FindAsync(id);
        if (aluguel is null) return NotFound();

        var validation = await ValidateRental(dto);
        if (validation is not null) return validation;

        aluguel.ClienteId = dto.ClienteId;
        aluguel.VeiculoId = dto.VeiculoId;
        aluguel.DataInicio = dto.DataInicio;
        aluguel.DataFimPrevista = dto.DataFimPrevista;
        aluguel.DataDevolucao = dto.DataDevolucao;
        aluguel.QuilometragemInicial = dto.QuilometragemInicial;
        aluguel.QuilometragemFinal = dto.QuilometragemFinal;
        aluguel.ValorDiaria = dto.ValorDiaria;
        aluguel.ValorTotal = dto.ValorTotal;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var aluguel = await _context.Alugueis.FindAsync(id);
        if (aluguel is null) return NotFound();

        _context.Alugueis.Remove(aluguel);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ActionResult?> ValidateRental(AluguelDto dto)
    {
        var clienteExists = await _context.Clientes.AnyAsync(x => x.Id == dto.ClienteId);
        var veiculoExists = await _context.Veiculos.AnyAsync(x => x.Id == dto.VeiculoId);
        if (!clienteExists || !veiculoExists)
            return BadRequest(new { erro = "ClienteId ou VeiculoId inválidos." });

        if (dto.DataFimPrevista < dto.DataInicio)
            return BadRequest(new { erro = "A data final prevista não pode ser menor que a data inicial." });

        if (dto.DataDevolucao.HasValue && dto.DataDevolucao.Value < dto.DataInicio)
            return BadRequest(new { erro = "A data de devolução não pode ser menor que a data inicial." });

        if (dto.QuilometragemFinal.HasValue && dto.QuilometragemFinal.Value < dto.QuilometragemInicial)
            return BadRequest(new { erro = "A quilometragem final não pode ser menor que a inicial." });

        return null;
    }
}
