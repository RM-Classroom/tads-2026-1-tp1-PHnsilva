using LocadoraVeiculosApi.Data;
using LocadoraVeiculosApi.DTOs;
using LocadoraVeiculosApi.Helpers;
using LocadoraVeiculosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly ApplicationContext _context;

    public VeiculosController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var veiculos = await _context.Veiculos
            .Include(x => x.Fabricante)
            .Include(x => x.CategoriaVeiculo)
            .OrderBy(x => x.Modelo)
            .Select(x => new
            {
                x.Id,
                x.Modelo,
                x.AnoFabricacao,
                x.QuilometragemAtual,
                x.Placa,
                x.FabricanteId,
                Fabricante = x.Fabricante!.Nome,
                x.CategoriaVeiculoId,
                Categoria = x.CategoriaVeiculo!.Nome
            })
            .ToListAsync();

        return Ok(veiculos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var veiculo = await _context.Veiculos
            .Include(x => x.Fabricante)
            .Include(x => x.CategoriaVeiculo)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Modelo,
                x.AnoFabricacao,
                x.QuilometragemAtual,
                x.Placa,
                x.FabricanteId,
                Fabricante = x.Fabricante!.Nome,
                x.CategoriaVeiculoId,
                Categoria = x.CategoriaVeiculo!.Nome
            })
            .FirstOrDefaultAsync();

        return veiculo is null ? NotFound() : Ok(veiculo);
    }

    [HttpPost]
    public async Task<ActionResult<VeiculoDto>> Create(VeiculoDto dto)
    {
        if (!await ExistsRelations(dto.FabricanteId, dto.CategoriaVeiculoId))
            return BadRequest(new { erro = "FabricanteId ou CategoriaVeiculoId inválidos." });

        var veiculo = new Veiculo
        {
            Modelo = dto.Modelo,
            AnoFabricacao = dto.AnoFabricacao,
            QuilometragemAtual = dto.QuilometragemAtual,
            Placa = dto.Placa,
            FabricanteId = dto.FabricanteId,
            CategoriaVeiculoId = dto.CategoriaVeiculoId
        };

        _context.Veiculos.Add(veiculo);

        try
        {
            await _context.SaveChangesAsync();
            dto.Id = veiculo.Id;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraint(ex))
        {
            return Conflict(new { erro = "Placa já cadastrada." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VeiculoDto dto)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null) return NotFound();
        if (!await ExistsRelations(dto.FabricanteId, dto.CategoriaVeiculoId))
            return BadRequest(new { erro = "FabricanteId ou CategoriaVeiculoId inválidos." });

        veiculo.Modelo = dto.Modelo;
        veiculo.AnoFabricacao = dto.AnoFabricacao;
        veiculo.QuilometragemAtual = dto.QuilometragemAtual;
        veiculo.Placa = dto.Placa;
        veiculo.FabricanteId = dto.FabricanteId;
        veiculo.CategoriaVeiculoId = dto.CategoriaVeiculoId;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsUniqueConstraint(ex))
        {
            return Conflict(new { erro = "Placa já cadastrada." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null) return NotFound();

        _context.Veiculos.Remove(veiculo);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyConstraint(ex))
        {
            return Conflict(new { erro = "Não é possível excluir o veículo porque existem aluguéis vinculados a ele." });
        }
    }

    private async Task<bool> ExistsRelations(int fabricanteId, int categoriaId)
    {
        var fabricanteExists = await _context.Fabricantes.AnyAsync(x => x.Id == fabricanteId);
        var categoriaExists = await _context.CategoriasVeiculo.AnyAsync(x => x.Id == categoriaId);
        return fabricanteExists && categoriaExists;
    }
}
