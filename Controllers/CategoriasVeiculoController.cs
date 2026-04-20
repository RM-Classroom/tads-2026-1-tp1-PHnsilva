using LocadoraVeiculosApi.Infrastructure.Data;
using LocadoraVeiculosApi.Application.DTOs;
using LocadoraVeiculosApi.Shared.Helpers;
using LocadoraVeiculosApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasVeiculoController : ControllerBase
{
    private readonly ApplicationContext _context;

    public CategoriasVeiculoController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaVeiculoDto>>> GetAll()
    {
        var categorias = await _context.CategoriasVeiculo
            .OrderBy(x => x.Nome)
            .Select(x => new CategoriaVeiculoDto
            {
                Id = x.Id,
                Nome = x.Nome,
                ValorDiariaBase = x.ValorDiariaBase
            })
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaVeiculoDto>> GetById(int id)
    {
        var categoria = await _context.CategoriasVeiculo
            .Where(x => x.Id == id)
            .Select(x => new CategoriaVeiculoDto
            {
                Id = x.Id,
                Nome = x.Nome,
                ValorDiariaBase = x.ValorDiariaBase
            })
            .FirstOrDefaultAsync();

        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaVeiculoDto>> Create(CategoriaVeiculoDto dto)
    {
        var categoria = new CategoriaVeiculo
        {
            Nome = dto.Nome,
            ValorDiariaBase = dto.ValorDiariaBase
        };

        _context.CategoriasVeiculo.Add(categoria);
        await _context.SaveChangesAsync();
        dto.Id = categoria.Id;
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoriaVeiculoDto dto)
    {
        var categoria = await _context.CategoriasVeiculo.FindAsync(id);
        if (categoria is null) return NotFound();

        categoria.Nome = dto.Nome;
        categoria.ValorDiariaBase = dto.ValorDiariaBase;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.CategoriasVeiculo.FindAsync(id);
        if (categoria is null) return NotFound();

        _context.CategoriasVeiculo.Remove(categoria);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyConstraint(ex))
        {
            return Conflict(new { erro = "Não é possível excluir a categoria porque existem veículos vinculados a ela." });
        }
    }
}
