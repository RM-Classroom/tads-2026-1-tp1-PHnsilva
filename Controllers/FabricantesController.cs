using LocadoraVeiculosApi.Infrastructure.Data;
using LocadoraVeiculosApi.Application.DTOs;
using LocadoraVeiculosApi.Shared.Helpers;
using LocadoraVeiculosApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FabricantesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public FabricantesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FabricanteDto>>> GetAll()
    {
        var fabricantes = await _context.Fabricantes
            .OrderBy(x => x.Nome)
            .Select(x => new FabricanteDto
            {
                Id = x.Id,
                Nome = x.Nome,
                PaisOrigem = x.PaisOrigem
            })
            .ToListAsync();

        return Ok(fabricantes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FabricanteDto>> GetById(int id)
    {
        var fabricante = await _context.Fabricantes
            .Where(x => x.Id == id)
            .Select(x => new FabricanteDto
            {
                Id = x.Id,
                Nome = x.Nome,
                PaisOrigem = x.PaisOrigem
            })
            .FirstOrDefaultAsync();

        return fabricante is null ? NotFound() : Ok(fabricante);
    }

    [HttpPost]
    public async Task<ActionResult<FabricanteDto>> Create(FabricanteDto dto)
    {
        var fabricante = new Fabricante
        {
            Nome = dto.Nome,
            PaisOrigem = dto.PaisOrigem
        };

        _context.Fabricantes.Add(fabricante);
        await _context.SaveChangesAsync();
        dto.Id = fabricante.Id;
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FabricanteDto dto)
    {
        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null) return NotFound();

        fabricante.Nome = dto.Nome;
        fabricante.PaisOrigem = dto.PaisOrigem;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null) return NotFound();

        _context.Fabricantes.Remove(fabricante);

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (DbExceptionHelper.IsForeignKeyConstraint(ex))
        {
            return Conflict(new { erro = "Não é possível excluir o fabricante porque existem veículos vinculados a ele." });
        }
    }
}
